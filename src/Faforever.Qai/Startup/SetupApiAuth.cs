using DSharpPlus;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Structures.Link;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Faforever.Qai.Startup
{
    public static partial class StartupExtensions
    {
        public static void SetupApiAuth(this IServiceCollection services, BotConfig botConfig)
        {
            var apiUrl = botConfig.Faf.Api;

            // Setup the OAuth2 settings
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = "FAF";
            })
                .AddCookie()
                .AddOAuth("FAF", options =>
                {
                    options.AuthorizationEndpoint = $"{apiUrl}oauth/authorize"; // FAF API Endpoint.

                    options.CallbackPath = new PathString("/authorization-code/callback"); // local auth endpoint
                    options.AccessDeniedPath = new PathString("/api/link/denied");

                    // Other FAF OAtuh configuration settings
                    options.ClientId = botConfig.Faf.ClientId;
                    options.ClientSecret = botConfig.Faf.ClientSecret;
                    options.TokenEndpoint = $"{apiUrl}oauth/token";

                    options.Scope.Add("public_profile");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            // Get the FAF user information
                            var req = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}me");

                            req.Headers.Authorization = new("Bearer", context.AccessToken);

                            var res = await context.Backchannel.SendAsync(req);

                            if (res.IsSuccessStatusCode)
                            { // if the request is valid, get the JSON data from it
                                var rawJson = await res.Content.ReadAsStreamAsync();

                                var faf = await System.Text.Json.JsonSerializer.DeserializeAsync<FafUser>(rawJson);

                                if (context.Request.Cookies.TryGetValue("token", out var token))
                                {
                                    var link = context.HttpContext.RequestServices.GetRequiredService<AccountLinkService>();

                                    try
                                    { // bind the information to the link with the token from the cookies
                                        link.BindFafUser(token, faf.Data.Attributes.UserId, faf.Data.Attributes.UserName);
                                    }
                                    catch (Exception ex)
                                    {
                                        context.Response.Cookies.Append("error", ex.Message);
                                    }

                                    context.Success();
                                }
                                else
                                {
                                    context.Response.Cookies.Append("error", "No token found.");
                                }
                            }
                            else
                            {
                                context.Response.Cookies.Append("error", "Failed to get user information from access token");
                            }
                        },
                        OnRemoteFailure = context =>
                        {
                            // TODO remove token from cookies and delete server token cache.
                            Console.WriteLine(context.Failure.Message);

                            return Task.CompletedTask;
                        }
                    };
                })
                // OAuth2 setup for authenticating with Discord.
                .AddOAuth("DISCORD", options =>
                {
                    options.AuthorizationEndpoint = $"{botConfig.Discord.Api}/oauth2/authorize";

                    options.CallbackPath = new PathString("/authorization-code/discord-callback"); // local auth endpoint
                    options.AccessDeniedPath = new PathString("/api/link/denied");

                    options.ClientId = botConfig.Discord.ClientId;
                    options.ClientSecret = botConfig.Discord.ClientSecret;
                    options.TokenEndpoint = $"{botConfig.Discord.TokenEndpoint}";

                    options.Scope.Add("identify");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            // get user data
                            var client = new DiscordRestClient(new()
                            {
                                Token = context.AccessToken,
                                TokenType = TokenType.Bearer
                            });

                            var user = await client.GetCurrentUserAsync();

                            if (context.Request.Cookies.TryGetValue("token", out var token))
                            {
                                var link = context.HttpContext.RequestServices.GetRequiredService<AccountLinkService>();

                                try
                                { // verify the user information grabbed matches the user info
                                  // saved from the inital command
                                    if (!link.VerifyDiscord(token, user.Id))
                                    {
                                        context.Response.Cookies.Append("error", "Discord ID used for sign in did not match Discord ID from the Discord Application.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    context.Response.Cookies.Append("error", ex.Message);
                                }

                                context.Success();
                            }
                            else
                            {
                                context.Response.Cookies.Append("error", "No token found.");
                            }
                        },
                        OnRemoteFailure = context =>
                        {
                            // TODO remove token from cookies and delete server token cache.

                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
