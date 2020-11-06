using System.Threading.Tasks;

using DSharpPlus.Entities;

using Qmmands;

namespace Faforever.Qai.Core.Commands
{
	public class DiscordOnlyQmmandsTestCommand : DiscordCommandModule
	{
		[Command("discordtest")]
		public async Task TestCommandAsync()
		{
			await Context.ReplyAsync("Test basic response.");
			await Context.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder().WithDescription("Test Embed."));
		}
	}
}
