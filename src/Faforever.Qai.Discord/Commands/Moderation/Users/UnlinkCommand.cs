using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Moderation.Users
{
	public class UnlinkCommand : CommandModule
	{
		[Command("unlink")]
		[Description("Breaks the link between a discord user and a FAF account.")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task UnlinkCommandAsync(CommandContext ctx, DiscordMember discordMember)
		{
			await RespondBasicError("Not implemented.");
		}

		[Command("unlink")]
		public async Task UnlinkCommandAsync(CommandContext ctx, string fafUsername)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
