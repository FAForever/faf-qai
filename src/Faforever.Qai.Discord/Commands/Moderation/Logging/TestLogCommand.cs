using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Logging
{
	public class TestLogCommand : CommandModule
	{
		[Command("testlog")]
		[Description("Tests the log channel on your server.")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task TestLogCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
