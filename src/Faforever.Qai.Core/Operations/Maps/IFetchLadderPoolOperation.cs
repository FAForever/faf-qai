using System.Collections.Generic;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Maps
{
    public interface IFetchLadderPoolOperation
    {
        public Task<IEnumerable<MapPool>> FetchLadderPoolAsync();
    }
}
