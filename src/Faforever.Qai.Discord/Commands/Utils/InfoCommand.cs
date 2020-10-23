using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Utils
{
	public class InfoCommand : CommandModule
	{
		[Command("info")]
		[Description("Gets bot info.")]
		[Aliases("botinfo")]
		public async Task InfoCommandAsync(CommandContext ctx)
		{
			await RespondBasicSuccess("This is a WIP version of Dostya!\n" +
				$"Connected to shard: {ctx.Client.ShardId}");

			// For more freedom with responses, use:
			// await ctx.RespondAsync();
		}
	}
}