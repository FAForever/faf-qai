using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Utils
{
	public class SubscribeCommand : CommandModule
	{
		[Command("subscribe")]
		[Description("Subscribe to a discord role that is registered via Dostya.")]
		public async Task SubscribeCommandAsync(CommandContext ctx,
			[Description(@"Role to subscribe to. Use `""` to surround the name of a multi word role.")]
			DiscordRole discordRole)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
