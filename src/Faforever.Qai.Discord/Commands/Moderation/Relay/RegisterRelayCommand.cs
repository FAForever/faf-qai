using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Relay
{
	public class RegisterRelayCommand : CommandModule
	{
		[Command("registerrelay")]
		[Description("Registers a link between an IRC channel and a Discord channel.")]
		[Aliases("rrealy")]
		[RequireUserPermissions(Permissions.ManageChannels)]
		public async Task ExampleCommandAsync(CommandContext ctx,
			[Description("Discord channel to link to.")]
			DiscordChannel discordChannel,
			
			[Description("IRC channel to link to.")]
			string ircChannel)
		{
			await RespondBasicError("Not Implmented");
		}
	}
}
