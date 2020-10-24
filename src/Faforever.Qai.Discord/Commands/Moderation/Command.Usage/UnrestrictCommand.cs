using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Command.Usage
{
	public class UnrestrictCommand : CommandModule
	{
		[Command("unrestrict")]
		[Description("Remove a command from the restrictions list.")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task UnrestrictCommandAsync(CommandContext ctx,
			[Description("Command to remove from the restrictions list.")]
			string commmandName)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
