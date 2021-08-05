using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Player
{
	public interface IFetchPlayerStatsOperation
	{
		Task<FetchPlayerStatsResult> FetchPlayer(string username);
	}
}