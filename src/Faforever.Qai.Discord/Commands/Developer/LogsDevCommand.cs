using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Developer
{
	public class LogsDevCommand : CommandModule
	{
		[Command("devlogs")]
		[Description("DMs the last bot logs to the user.")]
		[RequireOwner]
		public async Task DevLogsCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
