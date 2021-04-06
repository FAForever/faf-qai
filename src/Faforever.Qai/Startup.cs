using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using DSharpPlus;

using Faforever.Qai.Core;
using Faforever.Qai.Core.Commands.Arguments.Converters;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Operations.Clan;
using Faforever.Qai.Core.Operations.Clients;
using Faforever.Qai.Core.Operations.Content;
using Faforever.Qai.Core.Operations.Maps;
using Faforever.Qai.Core.Operations.Player;
using Faforever.Qai.Core.Operations.Replays;
using Faforever.Qai.Core.Operations.Units;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Services.BotFun;
using Faforever.Qai.Core.Structures.Configurations;
using Faforever.Qai.Core.Structures.Link;
using Faforever.Qai.Discord;
using Faforever.Qai.Discord.Core.Structures.Configurations;
using Faforever.Qai.Discord.Utils.Bot;
using Faforever.Qai.Irc;

using IrcDotNet;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;

using Qmmands;

using static Faforever.Qai.Core.Services.AccountLinkService;

namespace Faforever.Qai
{
	public class Startup
	{
		/// <summary>
		/// Base FAF API Url
		/// </summary>
		public static Uri ApiUri { get; private set; }

		public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
			ApiUri = new($"{Configuration["Config:Faf:Api"]}");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			// API/Website Config
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
				{
					Title = "Faforever.Qai", 
					Version = "v1",
					Description = "The API for the Faforever.Qai and Dostya bots."
				});
            });

			// Bot Services Config
			DatabaseConfiguration dbConfig = new()
			{
				DataSource = $"Data Source={Configuration["Config:DataSource"]}"
			};

#if DEBUG
			// For use when the DB is Database/db-name.db
			if (!Directory.Exists("Database"))
				Directory.CreateDirectory("Database");
#endif

			BotFunConfiguration botFunConfig;
			using (FileStream fs = new(Path.Join("Config", "games_config.json"), FileMode.Open))
			{
				using StreamReader sr = new(fs);
				var json = sr.ReadToEnd();
				botFunConfig = JsonConvert.DeserializeObject<BotFunConfiguration>(json);
			}

			TwitchClientConfig twitchCfg = new()
			{
				ClientId = Configuration["Config:TwitchClientId"],
				ClientSecret = Environment.GetEnvironmentVariable("TWITCH_CLIENT_SECRET")
			};

			services.AddLogging(options => options.AddConsole())
				.AddDbContext<QAIDatabaseModel>(options =>
				{
					options.UseSqlite(dbConfig.DataSource);
				}, ServiceLifetime.Singleton, ServiceLifetime.Singleton)
				.AddSingleton<RelayService>()
				// Command Service Registration
				.AddSingleton((x) =>
				{
					var options = new CommandService(new CommandServiceConfiguration()
					{
						// Additional configuration for the command service goes here.

					});

					// Command modules go here.
					options.AddModules(System.Reflection.Assembly.GetAssembly(typeof(CustomCommandContext)));
					// Argument converters go here.
					options.AddTypeParser(new DiscordChannelTypeConverter());
					options.AddTypeParser(new DiscordRoleTypeConverter());
					options.AddTypeParser(new BotUserCapsuleConverter());
					return options;
				})
				.AddSingleton<QCommandsHandler>()
				.AddSingleton(typeof(TwitchClientConfig), twitchCfg)
				// Operation Service Registration
				.AddSingleton<IBotFunService>(new BotFunService(botFunConfig))
				.AddSingleton<DiscordEventHandler>()
				.AddSingleton<AccountLinkService>()
				.AddTransient<IFetchPlayerStatsOperation, ApiFetchPlayerStatsOperation>()
				.AddTransient<IFindPlayerOperation, ApiFindPlayerOperation>()
				.AddTransient<ISearchUnitDatabaseOperation, UnitDbSearchUnitDatabaseOpeartion>()
				.AddTransient<IPlayerService, OperationPlayerService>()
				.AddTransient<ISearchMapOperation, ApiSearchMapOperation>()
				.AddTransient<IFetchLadderPoolOperation, ApiFetchLadderPoolOperation>()
				.AddTransient<IFetchReplayOperation, ApiFetchReplayOperation>()
				.AddTransient<IFetchClanOperation, ApiFetchClanOperation>()
				.AddTransient<IFetchTwitchStreamsOperation, FetchTwitchStreamsOperation>();
			// HTTP Client Mapping
			services.AddHttpClient<ApiClient>(client =>
			{
				client.BaseAddress = ApiUri;
			});

			services.AddHttpClient<UnitClient>(client =>
			{
				client.BaseAddress = new Uri(UnitDbUtils.UnitApi);
			});

			services.AddHttpClient<TwitchClient>();
			// Discord Information Setup
			DiscordBotConfiguration discordConfig = new()
			{
				Prefix = Configuration["Config:BotPrefix"],
				Shards = 1,
				Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
			};

			var dcfg = new DiscordConfiguration
			{
				Token = discordConfig.Token,
				TokenType = TokenType.Bot,
				MinimumLogLevel = LogLevel.Debug,
				ShardCount = discordConfig.Shards, // Default to 1 for automatic sharding.
				Intents = DiscordIntents.Guilds | DiscordIntents.GuildMessages,
			};

			services.AddSingleton(discordConfig)
				.AddSingleton<DiscordShardedClient>(x =>
				{
					return new(dcfg);
				})
				.AddSingleton<DiscordRestClient>(x =>
				{
					return new(dcfg);
				})
				.AddSingleton<DiscordBot>();
			// IRC Information Setup
			var user = Configuration["Config:Irc:User"];
			var pass = Environment.GetEnvironmentVariable("IRC_PASS");
			IrcConfiguration ircConfig = new()
			{
				Connection = Configuration["Config:Irc:Connection"],
				UserName = user,
				NickName = user,
				RealName = user,
				Password = pass
			};

			var ircConnInfo = new IrcUserRegistrationInfo
			{
				NickName = ircConfig.NickName,
				RealName = ircConfig.RealName,
				Password = ircConfig.Password,
				UserName = ircConfig.UserName
			};

			services.AddSingleton(ircConfig)
				.AddSingleton(ircConnInfo as IrcRegistrationInfo)
				.AddSingleton<QaIrc>();

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
					options.AuthorizationEndpoint = $"{ApiUri}oauth/authorize"; // FAF API Endpoint.

					options.CallbackPath = new PathString("/authorization-code/callback"); // local auth endpoint
					options.AccessDeniedPath = new PathString("/api/link/denied");

					// Other FAF OAtuh configuration settings
					options.ClientId = Configuration["Config:Faf:ClientId"];
					options.ClientSecret = Environment.GetEnvironmentVariable("FAF_CLIENT_SECRET");
					options.TokenEndpoint = $"{ApiUri}oauth/token";

					options.Scope.Add("public_profile");

					options.Events = new OAuthEvents
					{
						OnCreatingTicket = async context =>
						{
							// Get the FAF user information
							var req = new HttpRequestMessage(HttpMethod.Get, $"{ApiUri}me");

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
						OnRemoteFailure = async context =>
						{
							// TODO remove token from cookies and delete server token cache.
							Console.WriteLine(context.Failure.Message);
						}
					};
				})
				// OAuth2 setup for authenticating with Discord.
				.AddOAuth("DISCORD", options =>
				{
					options.AuthorizationEndpoint = $"{Configuration["Config:Discord:Api"]}/oauth2/authorize";

					options.CallbackPath = new PathString("/authorization-code/discord-callback"); // local auth endpoint
					options.AccessDeniedPath = new PathString("/api/link/denied");

					options.ClientId = Configuration["Config:Discord:ClientId"];
					options.ClientSecret = Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET");
					options.TokenEndpoint = $"{Configuration["Config:Discord:TokenEndpoint"]}";

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
						OnRemoteFailure = async context =>
						{
							// TODO remove token from cookies and delete server token cache.


						}
					};
				});
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
			var db = app.ApplicationServices.GetRequiredService<QAIDatabaseModel>();
			ApplyDatabaseMigrations(db);

			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			// Register Swagger API Documentation
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Faforever.Qai v1"));

			app.UseHttpsRedirection();

            app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

			StartClients(app).GetAwaiter().GetResult();
        }

		/// <summary>
		/// Applies any pending database migrations.
		/// </summary>
		/// <param name="database">Database Context to apply migrations for.</param>
		private static void ApplyDatabaseMigrations(DbContext database)
		{
			if (!(database.Database.GetPendingMigrations()).Any())
			{
				return;
			}

			database.Database.Migrate();
			database.SaveChanges();
		}

		/// <summary>
		/// Starts the IRC and Discord Bot clients using the IAppliactionBuilders service provider.
		/// </summary>
		/// <param name="app">An instance of IAppliactionBuilder with regiserted services.</param>
		/// <returns>The task for this operation.</returns>
		private static async Task StartClients(IApplicationBuilder app)
		{
			var ircBot = app.ApplicationServices.GetRequiredService<QaIrc>();
			
			ircBot.Run();

			var discordBot = app.ApplicationServices.GetRequiredService<DiscordBot>();

			await discordBot.InitializeAsync();
			await discordBot.StartAsync();
		}
	}
}
