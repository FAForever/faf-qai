using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Logging
{
	public class UnLogCommand : CommandModule
	{
		[Command("unlog")]
		[Description("Disables moderation logging for your server")]
		[Aliases("ulog-mod")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task UnLogCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
