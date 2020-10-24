using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Utils
{
	public class UnsubscribeCommand : CommandModule
	{
		[Command("unsubscribe")]
		[Description("Unsubscribe from a discord role that is registered via Dostya")]
		public async Task UnsubscribeCommandAsync(CommandContext ctx,
			[Description(@"Role to unsubscribee from. Use `""` to surround the name of a multi word role.")]
			DiscordRole discordRole)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
