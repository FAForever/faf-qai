using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation
{
	public class FixBridgeCommand : CommandModule
	{
		[Command("fixbridge")]
		[Description("Reinitializes the connection to all IRC brdiges established")]
		[Aliases("fixirc")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task FixBridgeCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
