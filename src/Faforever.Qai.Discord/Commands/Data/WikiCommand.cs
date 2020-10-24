using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Faforever.Qai.Discord.Commands.Utils;

namespace Faforever.Qai.Discord.Commands.Data
{
	public class WikiCommand : CommandModule
	{
		[Command("wiki")]
		[Description("Searches the FAForever wiki.")]
		public async Task WikiCommandAsync(CommandContext ctx,
			[Description("Search terms")]
			string serachTerms)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
