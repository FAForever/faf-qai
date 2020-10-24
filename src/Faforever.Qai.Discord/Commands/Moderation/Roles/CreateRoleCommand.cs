using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Roles
{
	public class CreateRoleCommand : CommandModule
	{
		[Command("createrole")]
		[Description("Create a role that is registered with Dostya and allows subscribing," +
			" or adds an exsiting role to Dostya to allow subscribing.")]
		[Aliases("crole", "registerrole")]
		[RequireUserPermissions(Permissions.ManageRoles)]
		[Priority(2)]
		public async Task RegisterRoleAsync(CommandContext ctx
			[Description("Role to add to Dostya")]
			DiscordRole role)
		{
			await RespondBasicError("Not implemented.");
		}

		[Command("createrole")]
		public async Task CreateRoleCommandAsync(CommandContext ctx,
			[Description("Name of the new role to create")]
			string roleName,
			
			[Description("Optional: Color of the new role")]
			DiscordColor? roleColor = null)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
