using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Maps
{
    public class ApiFetchLadderPoolOperation : IFetchLadderPoolOperation
    {
        private readonly FafApiClient _api;

        public ApiFetchLadderPoolOperation(FafApiClient api)
        {
            _api = api;
        }

        public async Task<IEnumerable<MapPool>> FetchLadderPoolAsync()
        {
            var fafApiQuery = new ApiQuery<MapPool>()
                .Include("mapPoolAssignments,mapVersions,mapVersions.map,matchmakerQueueMapPool,matchmakerQueueMapPool.matchmakerQueue");

            var mapPools = await _api.GetAsync(fafApiQuery) ?? new List<MapPool>();

            return mapPools.Where(mp => !mp.MatchmakerQueueMapPool.MatchmakerQueue.TechnicalName.Contains("tmm4v4_full_share")).ToArray();

            /*
            string json = await _api.Client.GetStringAsync("/data/mapPool?include=matchmakerQueueMapPool");

            var data = JObject.Parse(json);

            JToken? allData = data["included"];

            if (allData is null) return null;

            var maps = allData.Where(x => x["type"]?.ToString() == "map");
            var versions = allData.Where(x => x["type"]?.ToString() == "mapVersion");

            List<MapResult> res = new();
            foreach (var map in maps)
            {
                var included = versions.FirstOrDefault(x => x["id"]?.ToObject<long>() == map["relationships"]?["latestVersion"]?["data"]?["id"]?.ToObject<long>());

                if (included is null) continue;

                var size = included["attributes"]?["height"]?.ToObject<long>().GetMapSize() ?? 0;

                res.Add(new()
                {
                    Title = map["attributes"]?["displayName"]?.ToString(),
                    CreatedAt = map["attributes"]?["createTime"]?.ToObject<DateTime>(),
                    Id = map["id"]?.ToObject<long>() ?? 0,
                    Ranked = included["attributes"]?["ranked"]?.ToObject<bool>(),
                    DownloadUrl = included["attributes"]?["downloadUrl"]?.ToObject<Uri>(),
                    PreviewUrl = included["attributes"]?["thumbnailUrlLarge"]?.ToObject<Uri>(),
                    Description = included["attributes"]?["description"]?.ToString().RemoveBadContent(),
                    MaxPlayers = included["attributes"]?["maxPlayers"]?.ToObject<long>(),
                    Author = null,
                    Size = $"{size}x{size} km",
                    Version = map["attributes"]?["version"]?.ToObject<int>()
                });
            }
            

            return res;
            */
        }
    }
}
