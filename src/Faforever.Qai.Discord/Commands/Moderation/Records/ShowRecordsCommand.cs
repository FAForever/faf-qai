using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Records
{
	public class ShowRecordsCommand : CommandModule
	{
		[Command("showrecords")]
		[Description("List the active recorded messages.")]
		[Aliases("records")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task ShowRecordsCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
