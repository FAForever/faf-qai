using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Roles
{
	public class DeleteRoleCommand : CommandModule
	{
		[Command("deleterole")]
		[Description("Delete a role that is registered to Dostya." +
			" By default, this will attempt to delete the Discord role as well.")]
		[Aliases("delrole")]
		[RequireUserPermissions(Permissions.ManageRoles)]
		public async Task DeleteRoleCommandAsync(CommandContext ctx,
			[Description(@"Role to delete. Use `""` to surround the name of a multi word role.")]
			DiscordRole role,
			
			[Description("Attempt to delete the Discord Role as well?")]
			bool deleteDiscord = true) // delete ALL of discord!
		{
			await RespondBasicError("Not implemented.");
		}
	}
}