using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Users
{
	public class UserInfoCommand : CommandModule
	{
		[Command("userinfo")]
		[Description("Gets user information.")]
		[RequireUserPermissions(Permissions.ManageMessages)]
		public async Task UserInfoCommandAsync(CommandContext ctx,
			[Description("Discord user to get the information about")]
			DiscordMember discordMember)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
