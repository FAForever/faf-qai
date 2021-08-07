using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Services
{
	public class GameService
	{
		private readonly FafApiClient _api;

		public GameService(FafApiClient api)
		{
			this._api = api;
		}

		public async Task<Game?> FetchGame(long gameId)
		{
			var query = new ApiQuery<Game>()
				.Where("id", "==", gameId)
				.Limit(1);

			var games = await _api.GetAsync(query);

			return games.FirstOrDefault();
		}

		public async Task<Game?> FetchLastGame(string username)
		{
			var query = new ApiQuery<Game>()
				.Where("playerStats.player.login", "==", username)
				.Sort("-startTime")
				.Limit(1);

			var games = await _api.GetAsync(query);

			return games.FirstOrDefault();
		}
	}
}
