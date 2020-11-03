using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Users
{
	public class WarnCommand : CommandModule
	{
		[Command("warn")]
		[Description("Warns the user.")]
		[RequireUserPermissions(Permissions.ManageMessages)]
		public async Task WarnCommandAsync(CommandContext ctx,
			[Description("User to warn")]
			DiscordMember discordMember,

			[Description("Optional message to log and send to the user.")]
			string? message = null)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
