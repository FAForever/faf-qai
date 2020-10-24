using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Logging
{
	public class UnLogMapsHereCommand : CommandModule
	{
		[Command("unlogmapshere")]
		[Description("")]
		[Aliases("ulog-map")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task UnlogMapsHereCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
