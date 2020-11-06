using System;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Services;

namespace Faforever.Qai.Core
{
	public class SimpleCommandParser : ICommandParser
	{
		private readonly string _commandPrefix;
		private readonly IPlayerService _playerService;

		public SimpleCommandParser(string commandPrefix, IPlayerService playerService)
		{
			_commandPrefix = commandPrefix;
			_playerService = playerService;
		}

		public async Task HandleMessage(ICommandSource source, string message)
		{
			if (!message.StartsWith(_commandPrefix))
				return;

			message = message.Substring(1);

			string[] messageParts = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);
			string command = messageParts[0].ToLower();
			switch (command)
			{
				case "player":
				case "ratings":
					if (messageParts.Length <= 1)
					{
						//TODO Make proper help texts for commands
						await source.Respond("You need to supply a username!");
					}
					else
					{
						FetchPlayerStatsResult? playerStats = await _playerService.FetchPlayerStats(messageParts[1]);
						await source.Respond($"found player '{playerStats.Name}' with the following information:");
						await source.Respond($"1v1: rating '{playerStats.Rating1v1:F0}', ranked '{playerStats.Ranking1v1}'");
						await source.Respond($"Global: rating '{playerStats.RatingGlobal:F0}', ranked '{playerStats.RankingGlobal}'");
					}

					break;
			}
		}
	}
}