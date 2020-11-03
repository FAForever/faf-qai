using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Players
{
	public class SearchPlayerCommand : CommandModule
	{
		[Command("searchplayer")]
		[Description("Get a list of users whose username's match the serach term.")]
		[Aliases("searchp", "splayer")]
		public async Task SearchPlayerCommandAsync(CommandContext ctx,
			[Description("Terms to serach by.")]
			string serachTerms)
		{
			// TODO: Consider using interactivity for this command.
			await RespondBasicError("Not implemented.");
		}
	}
}
