using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Faforever.Qai.Discord.Commands.Utils;

namespace Faforever.Qai.Discord.Commands.Data
{
	public class ReplayCommand : CommandModule
	{
		[Command("replay")]
		[Description("Gets a replay résumé and a link for the given replay ID, or for the last game played by a given player.")]
		[RequireBotPermissions(Permissions.EmbedLinks)]
		public async Task ReplayCommandAsync(CommandContext ctx,
			[Description("The replay ID to look for.")]
			int repalyId)
		{
			await RespondBasicError("Not implemented.");
		}

		[Command("replay")]
		[Aliases("lastreplay")]
		public async Task ReplayCommandAsync(CommandContext ctx, 
			[Description("The FAF player name to get the last replay for.")]
			string fafPlayerName)
		{
			// Get last replay id then call above method.
			await ReplayCommandAsync(ctx, 0);
		}
	}
}
