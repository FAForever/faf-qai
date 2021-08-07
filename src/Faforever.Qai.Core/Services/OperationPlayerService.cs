using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;
using Faforever.Qai.Core.Operations.Player;

namespace Faforever.Qai.Core.Services
{
	[ExcludeFromCodeCoverage]
	public class OperationPlayerService : IPlayerService
	{
		private readonly FafApiClient _api;
		private readonly QAIDatabaseModel _db;
		private readonly GameService _gameService;
		private readonly IFetchPlayerStatsOperation _playerStatsOperation;
		private readonly IFindPlayerOperation _findPlayerOperation;

		public OperationPlayerService(QAIDatabaseModel db, FafApiClient api, GameService gameService, IFetchPlayerStatsOperation playerStatsOperation, IFindPlayerOperation findPlayerOperation)
		{
			_api = api;
			_db = db;
			_gameService = gameService;
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

		public async Task<LastSeenPlayerResult?> LastSeenPlayer(string username)
		{
			var lastGame = await _gameService.FetchLastGame(username);

			// If no last game was found we need to query the player directly
			Player? player = null;

			if(lastGame is null)
			{
				player = await FetchPlayer(username);
			} 
			else
			{
				player = lastGame.PlayerStats.FirstOrDefault(ps => ps.Player.Login.Equals(username, StringComparison.InvariantCultureIgnoreCase))?.Player;
			}

			if (player == null)
				return null;

			return new LastSeenPlayerResult
			{
				Username = player.Login,
				SeenFaf = player?.UpdateTime,
				SeenGame = lastGame?.EndTime ?? lastGame?.StartTime
			};
		}

		public async Task<Player?> FetchPlayer(string username)
		{
			var query = new ApiQuery<Player>()
				.Where("login", username)
				.Limit(1);

			var players = await _api.GetAsync(query);

			return players.FirstOrDefault();
		}
	}
}