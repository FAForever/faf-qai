using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

namespace Faforever.Qai.Irc.Commands
{
	public static class IrcOnlyTestCommand
	{
		public static async Task Execute(IRCCommandContext ctx)
		{
			await ctx.ReplyAsync("This is a test response.");
			ctx.Client.SendMessage(ctx.Name, "Another test response.");
		}
	}
}
