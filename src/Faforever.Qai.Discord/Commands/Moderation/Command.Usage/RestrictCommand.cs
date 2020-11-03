using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Moderation.Command.Usage
{
	public class RestrictCommand : CommandModule
	{
		[Command("restrict")]
		[Description("Prevents anyone from firing the command, except Mods")]
		[RequireUserPermissions(Permissions.ManageGuild)]
		public async Task RestrictCommandAsync(CommandContext ctx,
			[Description("Name of the command to restrict")]
			string commandName)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
