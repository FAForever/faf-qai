using System;
using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;
using Newtonsoft.Json.Linq;

namespace Faforever.Qai.Core.Operations.Maps
{
    public class ApiSearchMapOperation : ISearchMapOperation
    {
        private readonly FafApiClient _api;

        public ApiSearchMapOperation(FafApiClient api)
        {
            _api = api;
        }

        public async Task<Map?> GetMapAsync(string map)
        {
            var query = new ApiQuery<Map>()
                .Include("versions,author")
                .Where("displayName", map)
                .Sort("-gamesPlayed");

            var maps = await this._api.GetAsync(query);

            return maps.FirstOrDefault();
        }

        public async Task<Map?> GetMapAsync(int mapId)
        {
            var query = new ApiQuery<Map>()
                .Include("versions,author")
                .Where("id", mapId)
                .Sort("-gamesPlayed");

            var maps = await this._api.GetAsync(query);

            return maps.FirstOrDefault();
        }

        private MapResult? ParseStringData(string data)
        {
            var json = JObject.Parse(data);

            JToken? map = json["data"]?.First;

            if (map is null || map["type"]?.ToString() != "map") return null;

            var revision = map["relationships"]?["latestVersion"]?["data"]?["id"]?.ToObject<long>() ?? 0;

            JToken? included = json["included"]?.FirstOrDefault(x => x["id"]?.ToObject<long>() == revision);

            if (included is null) return null;

            JToken? player = json["included"]?.FirstOrDefault(x => x["type"]?.ToString() == "player");

            var size = included["attributes"]?["height"]?.ToObject<long>().GetMapSize() ?? 0;

            return new()
            {
                Title = map["attributes"]?["displayName"]?.ToString(),
                CreatedAt = map["attributes"]?["createTime"]?.ToObject<DateTime>(),
                Id = map["id"]?.ToObject<long>() ?? 0,
                Ranked = included["attributes"]?["ranked"]?.ToObject<bool>(),
                DownloadUrl = included["attributes"]?["downloadUrl"]?.ToObject<Uri>(),
                PreviewUrl = included["attributes"]?["thumbnailUrlLarge"]?.ToObject<Uri>(),
                Description = included["attributes"]?["description"]?.ToString().RemoveBadContent(),
                MaxPlayers = included["attributes"]?["maxPlayers"]?.ToObject<long>(),
                Author = player?["attributes"]?["login"]?.ToString(),
                Size = $"{size}x{size} km",
                Version = map["attributes"]?["version"]?.ToObject<int>()
            };
        }
    }
}
