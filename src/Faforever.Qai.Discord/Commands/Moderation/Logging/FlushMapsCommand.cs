using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Logging
{
	public class FlushMapsCommand : CommandModule
	{
		[Command("flushmaps")]
		[Description("Resets the alrady show maps list.")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task FlushMapsCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
