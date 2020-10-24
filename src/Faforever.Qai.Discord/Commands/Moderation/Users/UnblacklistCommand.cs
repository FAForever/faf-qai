using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Users
{
	public class UnblacklistCommand : CommandModule
	{
		[Command("unblacklist")]
		[Description("Removes a user from the servers blacklist")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task UnblacklistCommandAsync(CommandContext ctx,
			[Description("Discord user to remove from the blacklist.")]
			DiscordUser user)
			=> await UnblacklistCommandAsync(ctx, user.Id);

		[Command("unblacklist")]
		public async Task UnblacklistCommandAsync(CommandContext ctx, 
			[Description("User ID of the user to remove from the blacklist")]
			ulong userId)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
