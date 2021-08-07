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
    public class SetRoleForCompletedLinkCommand : DiscordCommandModule
    {
        private readonly IServiceProvider _services;

        public SetRoleForCompletedLinkCommand(IServiceProvider services)
        {
            _services = services;
        }

        [Command("linkrole", "roleforlinks")]
        [Description("The role to give a user after they have linked their account. Leave the role blank to disable this feature.")]
        [RequirePermissions(DSharpPlus.Permissions.ManageRoles)]
        public async Task SetRoleForCompletedLinkCommandAsync(DiscordRole? role = null)
        {
            var db = _services.GetRequiredService<QAIDatabaseModel>();
            var guild = await db.FindAsync<DiscordGuildConfiguration>(Context.Guild.Id);

            if (guild is null)
            {
                guild = new(Context.Guild.Id, "!");
                await db.AddAsync(guild);
                await db.SaveChangesAsync();
            }

            guild.RoleWhenLinked = role?.Id;

            await db.SaveChangesAsync();

            await Context.ReplyAsync($"The role {role?.Mention ?? "`no role`"} will now be given to any member who completes the linking process.");
        }
    }
}
