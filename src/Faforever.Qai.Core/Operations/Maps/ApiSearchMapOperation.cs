using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Maps
{
    public class ApiSearchMapOperation(FafApiClient api) : ISearchMapOperation
    {
        public async Task<Map?> GetMapAsync(string map)
        {
            var query = new ApiQuery<Map>()
                .Include("versions,author")
                .Where("displayName", WhereOp.Contains, map)
                .Sort("-gamesPlayed");
            var maps = await api.GetAsync(query);
            return maps.FirstOrDefault();
        }

        public async Task<Map?> GetMapAsync(int mapId)
        {
            var query = new ApiQuery<Map>()
                .Include("versions,author")
                .Where("id", mapId)
                .Sort("-gamesPlayed");

            var maps = await api.GetAsync(query);

            return maps.FirstOrDefault();
        }
    }
}
