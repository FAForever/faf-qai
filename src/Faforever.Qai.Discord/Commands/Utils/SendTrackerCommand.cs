using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Utils
{
	public class SendTrackerCommand : CommandModule
	{
		[Command("sendtracker")]
		[Description("DMs the tracker file to you.")]
		[Aliases("tracker")]
		public async Task SendTrackerCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
