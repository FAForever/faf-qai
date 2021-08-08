using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Content;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Content
{
    public class FetchTwitchStreamsCommand : DualCommandModule<TwitchStreamsResult>
    {
        private readonly IFetchTwitchStreamsOperation _twitch;

        public FetchTwitchStreamsCommand(IFetchTwitchStreamsOperation twitch)
        {
            _twitch = twitch;
        }

        [Command("streams", "stream")]
        [Description("Get's twitch streams for Supcom with the highest views")]
        public async Task GetStreamsCommandAsync()
        {
            var data = await _twitch.GetTwitchStreamsAsync();

            if (data is null || data.Streams.Count <= 0)
            {
                await Context.ReplyAsync("No streams found.");
                return;
            }

            await ReplyAsync(data);
        }

        public override async Task ReplyAsync(DiscordCommandContext ctx, TwitchStreamsResult data)
        {
            List<string> res = new();
            foreach (var s in data.Streams)
                res.Add($"{s.UserLogin}: [{s.Title}]({s.StreamLink})");

            var output = new DiscordEmbedBuilder()
                .WithColor(Context.DostyaRed)
                .WithTitle("Live Streams")
                .WithUrl("https://www.twitch.tv/directory/game/Supreme%20Commander%3A%20Forged%20Alliance")
                .WithDescription(string.Join("\n\n", res).Replace("*", "").Replace("_", "").Replace("`", ""));

            await ctx.Channel.SendMessageAsync(output);
        }

        public override async Task ReplyAsync(IrcCommandContext ctx, TwitchStreamsResult data)
        {
            List<string> res = new();
            foreach (var s in data.Streams)
                res.Add($"{s.UserLogin}: {s.StreamLink}");

            await ctx.ReplyAsync($"Live Streams: {string.Join(" | ", res)}");
        }
    }
}
