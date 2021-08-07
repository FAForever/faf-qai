using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;
using Faforever.Qai.Core.Operations.Replays;
using Faforever.Qai.Core.Services;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Replay
{
	public class FetchReplayCommand : DualCommandModule
	{
		private GameService _gameService;

		public FetchReplayCommand(GameService gameService)
		{
			_gameService = gameService;
		}

		[Command("replay")]
		public async Task FetchReplayCommandAsync(long gameId)
		{
			var game = await _gameService.FetchGame(gameId);

			await RespondToUser(game);
		}

		[Command("lastreplay")]
		public async Task FetchLastReplayCommandAsync(string username)
		{
			var data = await _gameService.FetchLastGame(username);

			await RespondToUser(data);
		}

		private async Task RespondToUser(Game? game)
		{
			if (game is null)
			{
				await ReplyAsync("Failed to get a replay by that ID");
				return;
			}

			await ReplyAsync(() => IrcResponse(game), () => DiscordResponse(game));
		}

		private DiscordEmbed DiscordResponse(Game game)
		{
			var embed = new DiscordEmbedBuilder();

			var description = new StringBuilder();

			embed
				.WithColor(Context.DostyaRed)
				.WithTitle($"Download replay #{game.Id}")
				.WithThumbnail(game.MapVersion.ThumbnailUrlLarge)
				.AddField("Map Info:", $"{game.MapVersion.Map.DisplayName}", true)
				.AddField("Start Time", game.StartTime.ToString("u"), true)
				.AddField("Duration", game.Duration?.ToString() ?? "-", true)
				.WithUrl(game.ReplayUrl)
				;

			var players = (game.PlayerStats ?? Enumerable.Empty<PlayerStats>()).OrderBy(s => s.StartSpot);
			var teams = new Dictionary<int, List<string>>();
			foreach (var p in players) {
				if (!teams.ContainsKey(p.Team))
					teams[p.Team] = new();

				teams[p.Team].Add($"{p.Player.Login} ({p.BeforeRating}, {p.FactionName})");
			}

			var n = 1;
			foreach (var team in teams.OrderBy(t => t.Key))
			{
				var field = "";

				foreach (var player in team.Value)
					field += $"{player}\n";

				embed.AddField($"Team {n}", field, true);

				n++;
			}

			embed.WithDescription(description.ToString());

			return embed;
		}

		public string IrcResponse(Game res)
		{
			var output = $"Replay #{res.Id}, {res.MapVersion.Map.DisplayName}, duration: {res.Duration}, {res.ReplayUrl}";

			return output;
		}
	}
}
