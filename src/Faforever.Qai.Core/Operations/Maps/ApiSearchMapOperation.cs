using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Operations.FafApi;

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
                .Where("displayName", WhereOp.Contains, map)
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
    }
}
