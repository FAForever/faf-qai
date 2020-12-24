using System.Threading.Tasks;

using DSharpPlus;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Utility
{
	public class AliveCommand : DualCommandModule
	{
		[Command("alive", "respond")]
		[Description("Sends a response message to the user. Differes depending on the command context.")]
		public async Task AliveCommandAsync()
		{
			string msg = "No valid context found.";
			if(Context is DiscordCommandContext dctx)
			{
				msg = $"Ping: {dctx.Client.Ping}ms";
			}
			else if (Context is IRCCommandContext ictx)
			{
				msg = $"Idle For: {ictx.Client.IdleDuration.TotalMilliseconds}ms";
			}

			await Context.ReplyAsync(msg);
		}
	}
}
