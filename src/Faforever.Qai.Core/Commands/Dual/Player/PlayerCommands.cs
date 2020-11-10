using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Services;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Player {
	public class PlayerCommands : DualCommandModule {
		private readonly IPlayerService _playerService;

		public PlayerCommands(IPlayerService playerService) {
			_playerService = playerService;
		}

		[Command("player", "ratings")]
		public async Task GetRatingsAsync(string username) {
			FetchPlayerStatsResult? playerStats = await _playerService.FetchPlayerStats(username);
			if (!await GetRatingsAsync_RespondDiscord(Context as DiscordCommandContext, playerStats))
				await Context.ReplyAsync($"found player '{playerStats.Name}' with the following information:\n" + 
					$"1v1: rating '{playerStats.LadderStats?.Rating.ToString("F0") ?? "0"}', ranked '{playerStats.LadderStats?.Ranking ?? 0}'\n" +
					$"Global: rating '{playerStats.GlobalStats?.Rating.ToString("F0") ?? "0"}', ranked '{playerStats.GlobalStats?.Ranking ?? 0}'");
		}

		private async Task<bool> GetRatingsAsync_RespondDiscord(DiscordCommandContext? ctx, FetchPlayerStatsResult? results)
		{
			if (ctx is null)
				return false; // cant do a thing if the context is null.

			if(results is null)
			{
				await ctx.ReplyAsync("Failed to find a player.");
				return true;
			}

			List<string> toJoin = new List<string>();
			if(results.OldNames.Count > 5)
			{
				toJoin = results.OldNames.GetRange(0, 5);
				toJoin.Add("...");
			}
			else
			{
				toJoin = results.OldNames;
			}


			var embed = new DiscordEmbedBuilder()
				.WithTitle(results.Name)
				.WithDescription($"**ID: {results.Id}**")
				.WithColor(DiscordColor.Red);

			if (toJoin.Count != 0)
				embed.AddField("Aliases", string.Join("\n", toJoin));

			if(!(results.LadderStats is null))
				embed.AddField("Ladder:", "```http\n" +
					$"Rating  :: {results.LadderStats?.Rating.ToString("F0") ?? "0"}\n" +
					$"Ranking :: {results.LadderStats?.Ranking ?? 0}\n" +
					$"Games   :: {results.LadderStats?.GamesPlayed ?? 0}\n" +
					"```");

			if (!(results.GlobalStats is null))
				embed.AddField("Global:", "```http\n" +
					$"Rating  :: {results.GlobalStats?.Rating.ToString("F0") ?? "0"}\n" +
					$"Ranking :: {results.GlobalStats?.Ranking ?? 0}\n" +
					$"Games   :: {results.GlobalStats?.GamesPlayed ?? 0}\n" +
					"```");

			if (!(results.Clan is null))
				embed.AddField($"Clan: {results.Clan?.Name}", "```http\n" +
					$"Clan Size :: {results.Clan?.Size ?? 0}\n" +
					$"URL       :: {results.Clan?.URL ?? "n/a"}\n" +
					"```");

			await ctx.Channel.SendMessageAsync(embed: embed);
			return true;
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