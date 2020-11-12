using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Player;

namespace Faforever.Qai.Core.Services
{
	[ExcludeFromCodeCoverage]
	public class OperationPlayerService : IPlayerService
	{
		private readonly IFetchPlayerStatsOperation _playerStatsOperation;
		private readonly IFindPlayerOperation _findPlayerOperation;

		public OperationPlayerService(IFetchPlayerStatsOperation playerStatsOperation, IFindPlayerOperation findPlayerOperation)
		{
			_playerStatsOperation = playerStatsOperation;
			_findPlayerOperation = findPlayerOperation;
		}

		public Task<FetchPlayerStatsResult> FetchPlayerStats(string username)
		{
			return _playerStatsOperation.FetchPlayer(username);
		}

		public Task<FindPlayerResult> FindPlayer(string searchTerm)
		{
			return _findPlayerOperation.FindPlayer(searchTerm);
		}
	}
}