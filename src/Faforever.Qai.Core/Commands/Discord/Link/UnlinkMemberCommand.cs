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
    public class UnlinkMemberCommand : DiscordCommandModule
    {
        private readonly IServiceProvider _services;

        public UnlinkMemberCommand(IServiceProvider services)
        {
            _services = services;
        }

        [Command("unlink")]
        [Description("Force unlink a member and their FAF account.")]
        [RequireFafStaff]
        public async Task UnlinkMemberLinkCommandAsync(DiscordUser user)
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
                db.Remove(link);
                await db.SaveChangesAsync();

                await Context.Client.SendMessageAsync(Context.Channel,
                    GetMemberLinkCommand.GetLinkDataEmbed(link)
                        .WithColor(Context.DostyaRed)
                        .WithTitle($"Old link for {user.Username}")
                        .WithAuthor("Unlinked account - displaying link details"));
            }
        }
    }
}
