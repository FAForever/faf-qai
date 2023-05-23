using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;
using Faforever.Qai.Core.Operations.Maps;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Map
{
    public class FetchLadderPoolCommand : DualCommandModule<MapPool>
    {
        private readonly IFetchLadderPoolOperation _ladder;

        public FetchLadderPoolCommand(IFetchLadderPoolOperation ladder)
        {
            _ladder = ladder;
        }

        [Command("pool", "ladder", "ladderpool")]
        [Description("Display the current ladder pool.")]
        public async Task FetchLadderPoolCommandAsync(string poolId)
        {
            var pools = await _ladder.FetchLadderPoolAsync() ?? new List<MapPool>();
            var pool = pools.FirstOrDefault(p => p.Id.ToString() == poolId);

            if (pool == null)
            {
                await ReplyAsync("No such map pool");
                return;
            }
                

            await ReplyAsync(pool);
        }

        public override async Task DiscordReplyAsync(DiscordCommandContext ctx, MapPool pool)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{pool.MatchmakerQueueMapPool.Name}")
                .WithColor(Context.DostyaRed)
                .WithThumbnail(pool.MapVersions[0].ThumbnailUrlSmall.Replace(" ", "%20") ?? "");

            int i = 0;
            foreach (var map in pool.MapVersions)
            {
                if (i++ >= 25)
                    break;

                embed.AddField(map.Map.DisplayName, map.Size, true);
            }


            await ctx.ReplyAsync(embed);
        }

        public override async Task IrcReplyAsync(IrcCommandContext ctx, MapPool pool)
        {
            string res = $"{pool.MatchmakerQueueMapPool.Name} Ladder Pool: ";
            List<string> stringData = new();
            int i = 0;

            foreach (var map in pool.MapVersions)
            {
                stringData.Add($"{map.Map.DisplayName} ({map.Size})");

                if (i++ >= 25)
                    break;
            }


            res += string.Join(", ", stringData);

            await ctx.ReplyAsync(res);
        }
    }
}
