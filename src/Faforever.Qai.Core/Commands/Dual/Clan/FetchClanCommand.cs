using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Clan;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Clan
{
    public class FetchClanCommand : DualCommandModule<FetchClanResult>
    {
        private readonly IFetchClanOperation _clan;

        public FetchClanCommand(IFetchClanOperation clan)
        {
            _clan = clan;
        }

        [Command("clan")]
        [Description("Get details about a clan.")]
        public async Task FetchClanCommandAsync(string clan)
        {
            var data = await _clan.FetchClanAsync(clan);

            if (data is null)
                await Context.ReplyAsync("Failed to get a clan by that tag or name.");
            else await ReplyAsync(data);
        }

        public override async Task DiscordReplyAsync(DiscordCommandContext ctx, FetchClanResult data)
        {
            var embed = new DiscordEmbedBuilder();
            var desc = !string.IsNullOrEmpty(data.Clan.Description) ? data.Clan.Description : "-";
            var size = data.Clan.Size ?? data.Members.Count;

            embed.WithTitle($"#{data.Clan.Id} {data.Clan.Name}")
                .WithUrl(data.Clan.URL)
                .WithColor(Context.DostyaRed)
                .AddField("Created", data.Clan.CreatedDate?.ToString("u"), true)
                .AddField("Clan Size", size.ToString(), true)
                .AddField("Description", desc, false);

            var c = 0;
            foreach (var member in data.Members)
            {
                if (++c == 21 && data.Members.Count > 21)
                {
                    embed.AddField("Other:", $"{data.Members.Count - (c - 1)} more not listed...", true);
                    break;
                }
                else
                    embed.AddField(member.Username, member.JoinDate?.ToShortDateString(), true);
            }

            await Context.ReplyAsync(embed);
        }

        public override async Task IrcReplyAsync(IrcCommandContext ctx, FetchClanResult data)
        {
            string res = $"Clan: {data.Clan.Name} ({data.Clan.URL}), Size: {data.Clan.Size}, Description: {data.Clan.Description?.Replace("\n", " ")}";

            await ctx.ReplyAsync(res);
        }
    }
}
