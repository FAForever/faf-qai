using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Http
{
    public class OAuthHandlerSettings
    {
        public string ClientId { get; set; } = "faf-website-public";
        public string ClientSecret { get; set; } = "banana";
        public string TokenEndpoint { get; set; } = "https://hydra.faforever.xyz/oauth2/token";
    }

    public class OAuthHandler : DelegatingHandler
    {
        private string accessToken = "";
        private DateTime tokenExpiry;
        private OAuthHandlerSettings settings;
        private readonly SemaphoreSlim semaphore = new(1, 1);


        public OAuthHandler(OAuthHandlerSettings settings)
        {
            /*
            if (string.IsNullOrEmpty(settings.ClientId))
                throw new ArgumentException("Client ID is required", nameof(settings.ClientId));

            if (string.IsNullOrEmpty(settings.ClientSecret))
                throw new ArgumentException("Client Secret is required", nameof(settings.ClientSecret));

            if (string.IsNullOrEmpty(settings.TokenEndpoint))
                throw new ArgumentException("Token Endpoint is required", nameof(settings.TokenEndpoint));
            */

            this.settings = settings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestUri = request.RequestUri?.ToString();
            if (requestUri is null)
                throw new InvalidOperationException("Request URI is null");

            await EnsureAccessTokenAsync(requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task EnsureAccessTokenAsync(string? requestUrl)
        {
            if (!string.IsNullOrEmpty(accessToken) && DateTime.Now < tokenExpiry)
                return;

            // Avoid recursion for token endpoint
            if (requestUrl == settings.TokenEndpoint)
                return;

            await semaphore.WaitAsync();

            try
            {
                if (!string.IsNullOrEmpty(accessToken) && DateTime.Now < tokenExpiry) // Double check inside lock
                    return;

                await FetchNewTokenAsync();
            }
            finally
            {
                semaphore.Release();
            }

            if (string.IsNullOrEmpty(accessToken))
                throw new InvalidOperationException("Could not fetch access token");
        }

        private async Task FetchNewTokenAsync()
        {
            var requestData = new Dictionary<string, string>
            {
                ["client_id"] = settings.ClientId,
                ["client_secret"] = settings.ClientSecret,
                ["grant_type"] = "client_credentials"
            };

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, settings.TokenEndpoint)
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            var response = await base.SendAsync(tokenRequest, default);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch the OAuth token. Status: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            if (tokenResponse != null)
            {
                accessToken = tokenResponse.AccessToken;
                int expiresIn = tokenResponse.ExpiresIn;
                tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 30); // Subtract a buffer to handle time discrepancies
            }
        }

        private class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; } = "";
            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonProperty("scope")]
            public string Scope { get; set; }  = "";
            [JsonProperty("token_type")]
            public string TokenType { get; set; } = "";
        }
    }
}
