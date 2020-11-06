using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Player;
using Faforever.Qai.Core.Services;

namespace Faforever.Qai.Core {
	[ExcludeFromCodeCoverage]
	public class OperationsPlayerService : IPlayerService {
		private readonly IFetchPlayerStatsOperation _playerStatsOperation;

		public OperationsPlayerService(IFetchPlayerStatsOperation playerStatsOperation) {
			_playerStatsOperation = playerStatsOperation;
		}

		public async Task<FetchPlayerStatsResult> FetchPlayerStats(string username) {
			return await _playerStatsOperation.FetchPlayer(username);
		}
	}
}