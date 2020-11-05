using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Player;

namespace Faforever.Qai.Core.Services {
	[ExcludeFromCodeCoverage]
	public class OperationPlayerService : IPlayerService {
		private readonly IFetchPlayerStatsOperation _playerStatsOperation;

		public OperationPlayerService(IFetchPlayerStatsOperation playerStatsOperation) {
			_playerStatsOperation = playerStatsOperation;
		}

		public Task<FetchPlayerStatsResult> FetchPlayerStats(string username) {
			return _playerStatsOperation.FetchPlayer(username );
		}
	}
}