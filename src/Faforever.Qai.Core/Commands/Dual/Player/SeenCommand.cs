using System.Threading.Tasks;
using Faforever.Qai.Core.Services;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Player
{
	public class SeenCommand : DualCommandModule
	{
		private IPlayerService _playerService;

		public SeenCommand(IPlayerService playerService)
		{
			this._playerService = playerService;
		}

		[Command("seen")]
		public async Task SeenPlayerAsync(string playerName)
		{
			var seen = await _playerService.LastSeenPlayer(playerName);

			if(seen == null)
			{
				await Context.ReplyAsync("No such player");
				return;
			}

			const string dateFormat = "yyyy-MM-dd HH:mm:ss";

			var fafDate = seen.SeenFaf?.ToString(dateFormat);
			var gameDate = seen.SeenGame?.ToString(dateFormat);

			string reply;
			string subString = "";
			if(fafDate is not null || gameDate is not null)
			{
				if (fafDate is not null)
					subString += $" and last login was {fafDate}";
				if (gameDate is not null)
					subString += $" and played a game {gameDate}";
			}

			if (!string.IsNullOrEmpty(subString))
				reply = $"{seen.Username} " + subString.Substring(5);
			else
				reply = $"{seen.Username} has never been seen";

			await Context.ReplyAsync(reply);
		}
	}
}