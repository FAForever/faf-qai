using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Users
{
	public class ShowLinksCommand : CommandModule
	{
		[Command("showlinks")]
		[Description("Displays all users with linked FAF accounts, and the accounts they are linked to.")]
		[Aliases("links")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task ShowLinksCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
