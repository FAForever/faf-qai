using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Utils
{
	public class LinkCommand : CommandModule
	{
		[Command("link")]
		[Description("Links the discord user account to the FAF account given.")]
		public async Task LinkCommandAsync(CommandContext ctx,
			[Description("FAF Player Name")]
			string fafPlayerName)
		{ // Is there no auth for this command?
			await RespondBasicError("Not implemented.");
		}
	}
}
