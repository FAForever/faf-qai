using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Faforever.Qai.Core.Clients
{
    public class TwitchClient
    {
        private readonly HttpClient _client;
        private readonly TwitchClientConfig _cfg;
        private string? BearerToken { get; set; }
        public TwitchClient(HttpClient client, TwitchClientConfig config)
        {
            _client = client;
            _cfg = config;
        }

        public async Task<string?> GetCurrentStreams(int gameId, bool isRepeat = false)
        {
            if (BearerToken is null)
            {
                await RefreshBearerToken();
                isRepeat = true;
            }

            var msg = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://api.twitch.tv/helix/streams?game_id={gameId}")
            };

            msg.Headers.Authorization = new("Bearer", BearerToken);
            msg.Headers.Add("Client-Id", _cfg.ClientId);

            var request = await _client.SendAsync(msg);

            if (request.IsSuccessStatusCode)
                return await request.Content.ReadAsStringAsync();
            else if (!isRepeat)
            {
                await RefreshBearerToken();
                return await GetCurrentStreams(gameId, true);
            }
            else
            { // failed requests twice in a row.
                return null;
            }
        }

        private async Task RefreshBearerToken()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var res = await _client.PostAsync($"https://id.twitch.tv/oauth2/token?client_id={_cfg.ClientId}&client_secret={_cfg.ClientSecret}&grant_type=client_credentials", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            if (res.IsSuccessStatusCode)
            {
                var resJson = await res.Content.ReadAsStringAsync();

                var json = JObject.Parse(resJson);
                BearerToken = json["access_token"]?.ToString();
            }
            else BearerToken = null;
        }
    }

    public struct TwitchClientConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
