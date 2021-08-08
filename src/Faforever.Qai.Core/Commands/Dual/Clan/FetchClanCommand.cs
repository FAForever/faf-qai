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

        public override async Task ReplyAsync(DiscordCommandContext ctx, FetchClanResult data)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithAuthor(data.Clan.Name, data.Clan.URL)
                .WithColor(Context.DostyaRed)
                .WithTitle($"ID: {data.Clan.Id}")
                .AddField("Created", data.Clan.CreatedDate?.ToString("u"), true)
                .AddField("URL", data.Clan.URL, true)
                .AddField("Clan Size", data.Clan.Size.ToString(), true)
                .AddField("Description", data.Clan.Description, false);

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

            await ctx.Channel.SendMessageAsync(embed);
        }

        public override async Task ReplyAsync(IrcCommandContext ctx, FetchClanResult data)
        {
            string res = $"Clan: {data.Clan.Name} ({data.Clan.URL}), Size: {data.Clan.Size}, Description: {data.Clan.Description?.Replace("\n", " ")}";

            await ctx.ReplyAsync(res);
        }
    }
}
