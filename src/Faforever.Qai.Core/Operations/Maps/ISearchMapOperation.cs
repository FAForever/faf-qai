using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Maps
{
    public interface ISearchMapOperation
    {
        public Task<Map?> GetMapAsync(string map);
        public Task<Map?> GetMapAsync(int mapId);
    }
}
