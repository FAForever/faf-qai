using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Logging
{
	public class LogHereCommand : CommandModule
	{
		[Command("loghere")]
		[Description("Sets the channel the command is run in for moderation logging.")]
		[Aliases("log-mod")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task LogHereCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
