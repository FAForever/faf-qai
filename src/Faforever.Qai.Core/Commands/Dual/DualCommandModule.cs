
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands
{
	public class DualCommandModule<T> : DualCommandModule
	{
		// Use this for specific command tools, such as default formatting, standard complex responses, etc.

		public async Task ReplyAsync(T data)
		{
			if (Context is DiscordCommandContext dctx)
				await ReplyAsync(dctx, data);
			else if (Context is IRCCommandContext ictx)
				await ReplyAsync(ictx, data);
		}

		public virtual async Task ReplyAsync(DiscordCommandContext ctx, T data)
			=> await ctx.Channel.SendMessageAsync(data?.ToString() ?? "No Data");

		public virtual async Task ReplyAsync(IRCCommandContext ctx, T data)
			=> await ctx.ReplyAsync(data?.ToString() ?? "No Data");
	}

	public class DualCommandModule : ModuleBase<CustomCommandContext>
	{

	}
}
