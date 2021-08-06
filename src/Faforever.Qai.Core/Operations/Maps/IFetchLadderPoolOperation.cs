using System.Collections.Generic;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Maps
{
	public interface IFetchLadderPoolOperation
	{
		public Task<IReadOnlyList<MapResult>?> FetchLadderPoolAsync();
	}
}
