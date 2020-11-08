using System.Threading.Tasks;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Services;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual {
	public class PlayerCommands : DualCommandModule {
		private readonly IPlayerService _playerService;

		public PlayerCommands(IPlayerService playerService) {
			_playerService = playerService;
		}

		[Command("player", "ratings")]
		public async Task GetRatingsAsync(string username) {
			FetchPlayerStatsResult? playerStats = await _playerService.FetchPlayerStats(username);
			await Context.ReplyAsync($"found player '{playerStats.Name}' with the following information:");
			await Context.ReplyAsync($"1v1: rating '{playerStats.Rating1v1:F0}', ranked '{playerStats.Ranking1v1}'");
			await Context.ReplyAsync(
				$"Global: rating '{playerStats.RatingGlobal:F0}', ranked '{playerStats.RankingGlobal}'");
		}

		[Command("searchplayer")]
		public async Task FindPlayerAsync(string searchTerm) {
			FindPlayerResult? findPlayerResult = await _playerService.FindPlayer(searchTerm);
			if (findPlayerResult.Usernames.Count == 0)
				await Context.ReplyAsync($"Found no players when searching for '{searchTerm}'");
			else {
				string players = string.Join(", ", findPlayerResult.Usernames);
				await Context.ReplyAsync($"Found the following players: {players}");
			}
		}
	}
}