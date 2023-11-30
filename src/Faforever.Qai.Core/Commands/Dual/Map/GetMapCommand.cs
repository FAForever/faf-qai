using System.Threading.Tasks;
using DSharpPlus.Entities;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Operations.Maps;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Map
{
    public class GetMapCommand : DualCommandModule<Operations.FafApi.Map>
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

        public override async Task DiscordReplyAsync(DiscordCommandContext ctx, Operations.FafApi.Map data)
        {
            var embed = new DiscordEmbedBuilder();
            var version = data.LatestVersion;

            embed.WithTitle("Download map")
                .WithColor(Context.DostyaRed)
                .WithUrl(version.DownloadUrl?.AbsoluteUri.Replace(" ", "%20"))
                .WithAuthor($"{version.Id} (ID #{data.Id})")
                .WithDescription(version.Description)
                .AddField("Size", version.Size, true)
                .AddField("Max Players", version.MaxPlayers.ToString(), true)
                .AddField("Ranked", version.Ranked.ToString(), true)
                .AddField("Created At", version.CreateTime.ToString("u"), true)
                .AddField("Author", data.Author?.Login ?? "Unknown")
                .WithImageUrl(version.ThumbnailUrlLarge?.AbsoluteUri.Replace(" ", "%20"));

            await Context.ReplyAsync(embed);
        }

        public override async Task IrcReplyAsync(IrcCommandContext ctx, Operations.FafApi.Map data)
        { 
            var latest = data.LatestVersion;

            await Context.ReplyAsync($"Map: {data.DisplayName}, ID: {data.Id}, Size: {latest.Size}," +
                $" Players: {latest.MaxPlayers}, Ranked: {latest.Ranked}, Author: {data.Author}," +
                $" Download: {latest.DownloadUrl?.AbsoluteUri.Replace(" ", "%20")}," +
                $" Preview: {latest.ThumbnailUrlLarge?.AbsoluteUri.Replace(" ", "%20")}");
        }
    }
}
