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
	public class GetMapCommand : DualCommandModule<MapResult>
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
					await ReplyAsync(res);
				else await Context.ReplyAsync("Failed to find a map by that ID.");
			}
			else
			{
				var res = await _map.GetMapAsync(map);

				if (res is not null)
					await ReplyAsync(res);
				else await Context.ReplyAsync("Failed to find a map by that name.");
			}
		}

		public override async Task ReplyAsync(DiscordCommandContext ctx, MapResult data)
		{
			var embed = new DiscordEmbedBuilder();
			embed.WithTitle("Download map")
				.WithColor(Context.DostyaRed)
				.WithUrl(data.DownloadUrl?.AbsoluteUri.Replace(" ", "%20"))
				.WithAuthor($"{data.Title} (ID #{data.Id})")
				.WithDescription(data.Description)
				.AddField("Size", data.Size, true)
				.AddField("Max Players", data.MaxPlayers.ToString(), true)
				.AddField("Ranked", data.Ranked.ToString(), true)
				.AddField("Created At", data.CreatedAt?.ToString("u"), true)
				.AddField("Author", data.Author)
				.WithImageUrl(data.PreviewUrl?.AbsoluteUri.Replace(" ", "%20"));

			await ctx.Channel.SendMessageAsync(embed);
		}

		public override async Task ReplyAsync(IRCCommandContext ctx, MapResult data)
			=> await ctx.ReplyAsync($"Map: {data.Title}, ID: {data.Id}, Size: {data.Size}," +
				$" Players: {data.MaxPlayers}, Ranked: {data.Ranked}, Author: {data.Author}," +
				$" Download: {data.DownloadUrl?.AbsoluteUri.Replace(" ", "%20")}," +
				$" Preview: {data.PreviewUrl?.AbsoluteUri.Replace(" ", "%20")}");
	}
}
