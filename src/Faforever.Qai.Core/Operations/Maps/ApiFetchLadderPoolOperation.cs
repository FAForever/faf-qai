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
        }
    }
}
