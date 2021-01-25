using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Maps;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Map
{
	public class FetchLadderPoolCommand : DualCommandModule
	{
		private readonly IFetchLadderPoolOperation _ladder;

		public FetchLadderPoolCommand(IFetchLadderPoolOperation ladder)
		{
			_ladder = ladder;
		}

		[Command("ladder", "pool", "ladderpool")]
		[Description("Display the current ladder pool.")]
		public async Task FetchLadderPoolCommandAsync()
		{
			var data = await _ladder.FetchLadderPoolAsync();

			if (data is null || data.Count <= 0)
				await Context.ReplyAsync("No map data found.");
			else if (Context is DiscordCommandContext dctx)
				await Respond(dctx, data);
			else if (Context is IRCCommandContext ictx)
				await Respond(ictx, data);
			else await Context.ReplyAsync("Failed to parse a command context.");
		}

		private async Task Respond(DiscordCommandContext ctx, IReadOnlyList<MapResult> maps)
		{
			var embed = new DiscordEmbedBuilder();
			embed.WithTitle("Showing Current Ladder Pool (First 25 Results)")
				.WithColor(Context.DostyaRed)
				.WithThumbnail(maps[0].PreviewUrl?.AbsoluteUri.Replace(" ", "%20") ?? "");

			int i = 0;
			foreach (var m in maps)
			{
				embed.AddField(m.Title, m.Size, true);

				if (i++ >= 25)
					break;
			}

			await ctx.Channel.SendMessageAsync(embed);
		}

		private async Task Respond(IRCCommandContext ctx, IReadOnlyList<MapResult> maps)
		{
			string res = $"Ladder Pool (First 25 Results): ";
			List<string> data = new();
			int i = 0;
			foreach (var m in maps)
			{
				data.Add($"{m.Title} ({m.Size})");

				if (i++ >= 25)
					break;
			}

			res += string.Join(", ", data);

			await ctx.ReplyAsync(res);
		}
	}
}
