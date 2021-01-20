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
	public class GetMapCommand : DualCommandModule
	{
		private readonly ISearchMapOperation _map;

		public GetMapCommand(ISearchMapOperation map)
		{
			_map = map;
		}

		[Command("map")]
		[Description("Get a map from the map database.")]
		public async Task GetMapCommandAsync([Remainder] string map)
		{
			if (int.TryParse(map, out var val))
			{
				var res = await _map.GetMapAsync(val);

				if (res is not null)
					await RespondAsync(res);
				else await Context.ReplyAsync("Failed to find a map by that ID.");
			}
			else
			{
				var res = await _map.GetMapAsync(map);

				if (res is not null)
					await RespondAsync(res);
				else await Context.ReplyAsync("Failed to find a map by that name.");
			}
		}

		private async Task RespondAsync(MapResult map)
		{
			var action = Context switch
			{
				DiscordCommandContext dctx => RespondDiscordAsync(dctx, map),
				IRCCommandContext irctx => RespondIrcAsync(irctx, map),
				_ => Context.ReplyAsync("Failed to get a proper context.")
			};

			await action;
		}

		private async Task RespondDiscordAsync(DiscordCommandContext ctx, MapResult map)
		{
			var embed = new DiscordEmbedBuilder();
			embed.WithTitle("Download map")
				.WithColor(Context.DostyaRed)
				.WithUrl(map.DownlaadUrl?.AbsoluteUri.Replace(" ", "%20"))
				.WithAuthor($"{map.Title} (ID #{map.Id})")
				.WithDescription(map.Description)
				.AddField("Size", map.Size, true)
				.AddField("Max Players", map.MaxPlayers.ToString(), true)
				.AddField("Ranked", map.Ranked.ToString(), true)
				.AddField("Created At", map.CreatedAt?.ToString("u"), true)
				.AddField("Author", map.Author)
				.WithImageUrl(map.PreviewUrl?.AbsoluteUri.Replace(" ", "%20"));

			await ctx.Channel.SendMessageAsync(embed);
		}

		private async Task RespondIrcAsync(IRCCommandContext ctx, MapResult map)
			=> await ctx.ReplyAsync($"Map: {map.Title}, ID: {map.Id}, Size: {map.Size}," +
				$" Players: {map.MaxPlayers}, Ranked: {map.Ranked}, Author: {map.Author}," +
				$" Download: {map.DownlaadUrl?.AbsoluteUri.Replace(" ", "%20")}," +
				$" Preview: {map.PreviewUrl?.AbsoluteUri.Replace(" ", "%20")}");
	}
}
