using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Authorization;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Database.Entities;

using Microsoft.Extensions.DependencyInjection;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Discord.Role
{
    public partial class RoleCommands : DiscordCommandModule
    {
        [Command("subscribe")]
        [Description("Subscribe to a role!")]
        [RequireBotPermissions(DSharpPlus.Permissions.ManageRoles)]
        public async Task SubscribeToRoleCommandAsync([Remainder] string roleName)
        {
            var lowerName = roleName.ToLower();
            DiscordRole? role = (from pair in Context.Guild.Roles
                                 where pair.Value.Name.ToLower().Equals(lowerName)
                                 select pair.Value).FirstOrDefault();

            if (role is null)
            {
                await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.DarkRed)
                    .WithDescription("No roles with that name were found on this server!"));
                return;
            }

            var db = _services.GetRequiredService<QAIDatabaseModel>();
            var guild = await db.FindAsync<DiscordGuildConfiguration>(Context.Guild.Id);

            if (guild is not null && guild.IsRoleSubscribable(role))
            {
                var member = await Context.Guild.GetMemberAsync(Context.User.Id);
                await member.GrantRoleAsync(role, "Member subscribed to role.");
                await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
                    .WithColor(Context.DostyaRed)
                    .WithDescription($"You now have the role {role.Mention}!"));
            }
            else
            {
                await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.DarkRed)
                    .WithDescription("The role you chose is not subscribable!"));
            }
        }

        [Command("unsubscribe")]
        [Description("Unsubscribe from a role!")]
        [RequireBotPermissions(DSharpPlus.Permissions.ManageRoles)]
        public async Task UnSubscribeToRoleCommandAsync([Remainder] string roleName)
        {
            var lowerName = roleName.ToLower();
            var member = await Context.Guild.GetMemberAsync(Context.User.Id);
            DiscordRole? role = (from r in member.Roles
                                 where r.Name.ToLower().Equals(lowerName)
                                 select r).FirstOrDefault();

            if (role is null)
            {
                await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.DarkRed)
                    .WithDescription("You do not have the selected role!"));
                return;
            }

            var db = _services.GetRequiredService<QAIDatabaseModel>();
            var guild = await db.FindAsync<DiscordGuildConfiguration>(Context.Guild.Id);

            if (guild is not null && guild.IsRoleSubscribable(role))
            {
                await member.RevokeRoleAsync(role, "Member unsubscribed from role.");
                await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
                    .WithColor(Context.DostyaRed)
                    .WithDescription($"You have unsubscribed from {role.Mention}!"));
            }
            else
            {
                await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
                    .WithColor(Context.DostyaRed)
                    .WithDescription($"You can't remove {role.Mention} with this command!"));
            }
        }
    }
}
