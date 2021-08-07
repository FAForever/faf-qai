using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Maps;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Map
{
    public class FetchLadderPoolCommand : DualCommandModule<IReadOnlyList<MapResult>>
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
            else await ReplyAsync(data);
        }

        public override async Task ReplyAsync(DiscordCommandContext ctx, IReadOnlyList<MapResult> data)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithTitle("Showing Current Ladder Pool (First 25 Results)")
                .WithColor(Context.DostyaRed)
                .WithThumbnail(data[0].PreviewUrl?.AbsoluteUri.Replace(" ", "%20") ?? "");

            int i = 0;
            foreach (var m in data)
            {
                embed.AddField(m.Title, m.Size, true);

                if (i++ >= 25)
                    break;
            }

            await ctx.Channel.SendMessageAsync(embed);
        }

        public override async Task ReplyAsync(IRCCommandContext ctx, IReadOnlyList<MapResult> data)
        {
            string res = $"Ladder Pool (First 25 Results): ";
            List<string> stringData = new();
            int i = 0;
            foreach (var m in data)
            {
                stringData.Add($"{m.Title} ({m.Size})");

                if (i++ >= 25)
                    break;
            }

            res += string.Join(", ", stringData);

            await ctx.ReplyAsync(res);
        }
    }
}
