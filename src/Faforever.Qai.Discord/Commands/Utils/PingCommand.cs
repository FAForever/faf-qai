using System.Diagnostics;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Utils
{
	public class PingCommand : CommandModule
	{
		[Command("ping")]
		[Description("Test the bot's response time.")]
		[Aliases("alive", "respond")]
		public async Task PingCommandAsync(CommandContext ctx)
		{
			Stopwatch timer = new Stopwatch();
			var pingEmbed = SuccessBase().WithTitle($"Ping for Shard {ctx.Client.ShardId}");
			pingEmbed.AddField("WS Latency:", $"{ctx.Client.Ping}ms");
			timer.Start();
			DiscordMessage msg = await ctx.RespondAsync(pingEmbed);
			await msg.ModifyAsync(null, pingEmbed.AddField("Response Time: (:ping_pong:)", $"{timer.ElapsedMilliseconds}ms").Build());
			timer.Stop();
		}
	}
}
