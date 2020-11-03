using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Users
{
	public class BlacklistCommand : CommandModule
	{
		[Command("blacklist")]
		[Description("If a ueser is specified, it prevents a user to use commands on this guild." +
			" Otherwise, it DMs a list of blacklisted users for this server")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task BlacklistCommandAsync(CommandContext ctx,
			[Description("User to blacklist")]
			DiscordUser user)
			=> await BlacklistCommandAsync(ctx, user.Id);

		[Command("blacklist")]
		public async Task BlacklistCommandAsync(CommandContext ctx,
			[Description("ID of the user to blacklsit")]
			ulong userId)
		{
			await RespondBasicError("Not implemented.");
		}

		[Command("blacklist")]
		[Priority(2)]
		public async Task GetBlacklistCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
