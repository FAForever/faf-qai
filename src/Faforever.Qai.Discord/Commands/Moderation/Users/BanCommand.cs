using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Users
{
	public class BanCommand : CommandModule
	{
		[Command("ban")]
		[Description("Bans a users from your server.")]
		[RequireUserPermissions(Permissions.BanMembers)]
		public async Task BanCommandAsync(CommandContext ctx,
			[Description("User to ban")]
			DiscordMember discordMember,

			[Description("Optional message to log and send the user")]
			string? message = null,

			[Description("Time to ban the user for.")]
			TimeSpan? banLength = null)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}