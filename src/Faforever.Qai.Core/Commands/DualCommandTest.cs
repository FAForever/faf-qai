using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

namespace Faforever.Qai.Core.Commands
{
	public static class DualCommandTest
	{
		public static async Task Execute(CustomCommandContext ctx)
		{
			await ctx.ReplyAsync("This is a test.");
		}
	}
}
