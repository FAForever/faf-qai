using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Logging
{
	public class LogMapsHereCommand : CommandModule
	{
		[Command("logmapshere")]
		[Description("Adds the channel to the map watching list - new maps will" +
			" automatically be shown in this channel whenever they're uplaoded.")]
		[Aliases("log-map")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task LogMapsHereCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}