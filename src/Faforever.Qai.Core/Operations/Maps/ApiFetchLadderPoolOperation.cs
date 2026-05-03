using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Maps
{
    public class ApiFetchLadderPoolOperation(FafApiClient api) : IFetchLadderPoolOperation
    {
        public async Task<IEnumerable<MapPool>> FetchLadderPoolAsync()
        {
            var fafApiQuery = new ApiQuery<MapPool>()
                .Include("mapPoolAssignments,mapVersions,mapVersions.map,matchmakerQueueMapPool,matchmakerQueueMapPool.matchmakerQueue");

            var mapPools = await api.GetAsync(fafApiQuery) ?? new List<MapPool>();

            return mapPools.Where(mp => !mp.MatchmakerQueueMapPool.MatchmakerQueue.TechnicalName.Contains("tmm4v4_full_share")).ToArray();
        }
    }
}
