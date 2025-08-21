using System;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using Faforever.Qai.Core.Commands.Authorization;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Database.Entities;

using Microsoft.Extensions.DependencyInjection;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Discord.Link
{
    public class GetMemberLinkCommand : DiscordCommandModule
    {
        private readonly IServiceProvider _services;

        public GetMemberLinkCommand(IServiceProvider services)
        {
            _services = services;
        }

        [Command("links", "getlinks")]
        [Description("Get the FAF account link for a Discord user")]
        [RequireFafStaff]
        public async Task GetMemberLinkCommandAsync(DiscordUser user)
        {
            var db = _services.GetRequiredService<QAIDatabaseModel>();
            var link = await db.FindAsync<AccountLink>(user.Id);

            if (link is null)
            {
                await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.DarkRed)
                    .WithDescription($"No link found for this member."));
            }
            else
            {
                await Context.Client.SendMessageAsync(Context.Channel, GetLinkDataEmbed(link)
                        .WithColor(Context.DostyaRed));
            }
        }

        public static DiscordEmbedBuilder GetLinkDataEmbed(AccountLink link)
        {
            return new DiscordEmbedBuilder()
                    .WithTitle($"Link for {link.DiscordUsername}")
                    .AddField("Discord", $"```\n" +
                    $"Username : {link.DiscordUsername}\n" +
                    $"ID       : {link.DiscordId}" +
                    $"\n```", true)
                    .AddField("FAF", $"```\n" +
                    $"Username : {link.FafUsername}\n" +
                    $"ID       : {link.FafId}" +
                    $"\n```", true);
        }
    }
}
