using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Faforever.Qai.Core.Services;

namespace Faforever.Qai.Discord.Commands.Moderation.Relay
{
	public class RegisterRelayCommand : CommandModule
	{
		private readonly RelayService _relay;

		public RegisterRelayCommand(RelayService relay)
		{
			this._relay = relay;
		}

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
			if (await _relay.AddRelayAsync(ctx.Guild.Id, discordChannel.Id, ircChannel))
			{
				await RespondBasicSuccess("Relay added!");
			}
			else
			{
				await RespondBasicError("Failed to add new relay.");
			}
		}
	}
}
