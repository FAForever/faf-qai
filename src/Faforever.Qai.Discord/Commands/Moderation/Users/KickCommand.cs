using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Users
{
	public class KickCommand : CommandModule
	{
		[Command("kick")]
		[Description("Kicks the user from your server.")]
		[RequireUserPermissions(Permissions.KickMembers)]
		public async Task KickCommandAsync(CommandContext ctx,
			[Description("User to kick from the server.")]
			DiscordMember discordMember,
			
			[Description("Optional message to send the user.")]
			string? message = null)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
