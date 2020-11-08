using System.Threading.Tasks;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Services;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual {
	public class DualCommandTest : DualCommandModule {
		private readonly IPlayerService _playerService;

		public DualCommandTest(IPlayerService playerService) {
			_playerService = playerService;
		}

		[Command("dualtest")]
		public async Task DualTestCommandAsync() {
			await Context.ReplyAsync("This is a test.");
		}

		[Command("player", "ratings")]
		public async Task GetRatingsAsync(string username) {
			FetchPlayerStatsResult? playerStats = await _playerService.FetchPlayerStats(username);
			await Context.ReplyAsync($"found player '{playerStats.Name}' with the following information:");
			await Context.ReplyAsync($"1v1: rating '{playerStats.Rating1v1:F0}', ranked '{playerStats.Ranking1v1}'");
			await Context.ReplyAsync(
				$"Global: rating '{playerStats.RatingGlobal:F0}', ranked '{playerStats.RankingGlobal}'");
		}
	}
}