using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Faforever.Qai.Discord.Commands.Utils;

namespace Faforever.Qai.Discord.Commands.Players
{
	public class PlayerRaitingsCommand : CommandModule
	{
		[Command("playerraitings")]
		[Description("Displays a player's avatar and rating information.")]
		[Aliases("player", "ratings")]
		[RequireBotPermissions(Permissions.EmbedLinks)]
		public async Task PlayerRaitingsCommandAsync(CommandContext ctx,
			[Description("Player to get raitings for.")]
			string fafPlayerName)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
