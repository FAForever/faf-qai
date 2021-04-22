using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Authorization;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Structures.Configurations;

using Microsoft.Extensions.DependencyInjection;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Discord.Role
{
	public partial class RoleCommands : DiscordCommandModule
	{
		[Command("registerrole")]
		[Description("Register a new role for users to subscribe to.")]
		[RequirePermissions(DSharpPlus.Permissions.ManageRoles)]
		public async Task RegisterRoleCommandAsync(DiscordRole role)
		{
			var db = _services.GetRequiredService<QAIDatabaseModel>();
			var guild = await db.FindAsync<DiscordGuildConfiguration>(Context.Guild.Id);

			if(guild is null)
			{
				guild = new()
				{
					Prefix = Context.Prefix,
					GuildId = Context.Guild.Id
				};

				await db.AddAsync(guild);
				await db.SaveChangesAsync();
			}

			if(guild.RegisterRole(role))
			{
				db.Update(guild);
				await db.SaveChangesAsync();

				await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
					.WithColor(Context.DostyaRed)
					.WithDescription($"{role.Mention} has been registered. It can now be subscribed to."));
			}
			else
			{
				await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
					.WithColor(DiscordColor.DarkRed)
					.WithDescription($"{role.Mention} has already been registered." +
					$" If you are looking to remove this role, use `{Context.Prefix}unregister {role.Id}`"));
			}
		}

		[Command("unregisterrole")]
		[Description("Unregister a role, preventing users from subscribing to it.")]
		[RequirePermissions(DSharpPlus.Permissions.ManageRoles)]
		public async Task UnRegisterRoleCommandAsync(DiscordRole role)
		{
			var db = _services.GetRequiredService<QAIDatabaseModel>();
			var guild = await db.FindAsync<DiscordGuildConfiguration>(Context.Guild.Id);

			if (guild is null)
			{
				guild = new()
				{
					Prefix = Context.Prefix,
					GuildId = Context.Guild.Id
				};

				await db.AddAsync(guild);
				await db.SaveChangesAsync();
			}

			if (guild.UnregisterRole(role))
			{
				db.Update(guild);
				await db.SaveChangesAsync();

				await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
					.WithColor(Context.DostyaRed)
					.WithDescription($"{role.Mention} was unregistered. It can no longer be subscribed to."));
			}
			else
			{
				await Context.Client.SendMessageAsync(Context.Channel, new DiscordEmbedBuilder()
					.WithColor(DiscordColor.DarkRed)
					.WithDescription($"{role.Mention} has not been registered." +
					$" If you are looking to register this role, use `{Context.Prefix}register {role.Id}`"));
			}
		}
	}
}
