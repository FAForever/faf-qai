using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Records
{
	public class RecordCommand : CommandModule
	{
		[Command("record")]
		[Description("Rcords a predefined message that will show up when a users uses the designated command." +
			" Leaving the message blank will return the command to its normal functionality.")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task RecordCommandAsync(CommandContext ctx, 
			[Description("Command name to set a message for.")]
			string commandName,
			
			[Description("Message to display when the command is run.")]
			string message)
		{
			await RespondBasicError("Not implemented.");
		}

		[Command("record")]
		public async Task RecordCommandAsync(CommandContext ctx,
			[Description("Command name to clear a message for")]
			string message)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
