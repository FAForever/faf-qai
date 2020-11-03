using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Utils
{
	public class RestrictionsCommand : CommandModule
	{
		[Command("restrictions")]
		[Description("DMs the restricted commands list to the user")]
		public async Task RestrictionsCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
