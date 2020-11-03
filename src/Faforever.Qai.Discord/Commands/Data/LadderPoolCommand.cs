using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Data
{
	public class LadderPoolCommand : CommandModule
	{
		[Command("ladderpool")]
		[Description("Gets the current 1v1 ladder map pool.")]
		[Aliases("ladder", "pool")]
		[RequireBotPermissions(Permissions.EmbedLinks)]
		public async Task LadderPoolCommandAsync(CommandContext ctx)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}
