using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Replays;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Replay
{
	public class FetchReplayCommand : DualCommandModule
	{
		private readonly IFetchReplayOperation _replay;

		public FetchReplayCommand(IFetchReplayOperation replay)
		{
			_replay = replay;
		}

		[Command("replay")]
		public async Task FetchReplayCommandAsync(string replayId)
		{
			var data = await _replay.FetchReplayAsync(replayId);

			await RespondToUser(data);
		}

		[Command("lastreplay")]
		public async Task FetchLastReplayCommandAsync(string username)
		{
			var data = await _replay.FetchLastReplayAsync(username);

			await RespondToUser(data);
		}

		private async Task RespondToUser(ReplayResult? data)
		{
			if (data is null)
				await Context.ReplyAsync("Failed to get a replay by that ID");
			else if (Context is DiscordCommandContext dctx)
				await Reply(dctx, data);
			else if (Context is IRCCommandContext ictx)
				await Reply(ictx, data);
			else await Context.ReplyAsync("Failed to parse a command context");
		}

		private async Task Reply(DiscordCommandContext ctx, ReplayResult res)
		{
			var embed = new DiscordEmbedBuilder();
			embed.WithAuthor(res.Title)
				.WithTitle($"Downlaod replay #{res.Id}")
				.WithUrl(res.ReplayUri?.AbsoluteUri.Replace(" ", "%20"))
				.WithThumbnail(res.MapInfo?.PreviewUrl?.AbsoluteUri.Replace(" ", "%20"))
				.AddField("Start Time", res.StartTime?.ToString("u"), true)
				.AddField("Victory Condition", res.VictoryConditions, true)
				.AddField("Validity", res.Validity, true)
				.AddField("Map Is Ranked?", res.MapInfo?.Ranked?.ToString() ?? "n/a", true)
				.AddField("Map Info:", $"{res.MapInfo?.Title} [{res.MapInfo?.Version}] ({res.MapInfo?.Size})");

			foreach(var p in res.PlayerData ?? Enumerable.Empty<FetchPlayerStatsResult>())
				embed.AddField($"{p.ReplayData?.Faction} - {p.Name}" +
					$" [{(res.Ranked1v1 ? "R" : "G")}{(res.Ranked1v1 ? p.LadderStats?.Rating : p.GlobalStats?.Rating)}]",
					$"Team {p.ReplayData?.Team}\nScore: {p.ReplayData?.Score}", true);

			await ctx.Channel.SendMessageAsync(embed);
		}

		private async Task Reply(IRCCommandContext ctx, ReplayResult res)
		{
			var output = $"Replay #{res.Id}: {res.Title} ({res.ReplayUri}) on " +
				$"{res.MapInfo?.Title} [{res.MapInfo?.Version}] ({res.MapInfo?.Size}). Players: ";
			List<string> pdata = new();

			foreach (var p in res.PlayerData ?? Enumerable.Empty<FetchPlayerStatsResult>())
				pdata.Add($"{p.Name} (T: {p.ReplayData?.Team}, S: {p.ReplayData?.Score})");

			output += string.Join(", ", pdata);

			await ctx.ReplyAsync(output);
		}
	}
}
