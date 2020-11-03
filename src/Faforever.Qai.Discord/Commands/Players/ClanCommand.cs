using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Players
{
	public class ClanCommand : CommandModule
	{
		[Command("clan")]
		[Description("Gets information about a Clan.")]
		public async Task ClanCommandAsync(CommandContext ctx,
			[Description("The clan tag, or name, to serach for.")]
			string fafClanTag)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
