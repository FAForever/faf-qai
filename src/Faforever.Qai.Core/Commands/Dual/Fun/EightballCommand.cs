using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Services.BotFun;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Fun
{
	public class EightballCommand : DualCommandModule
	{
		private readonly IBotFunService _botFun;

		public EightballCommand(IBotFunService botFun)
		{
			_botFun = botFun;
		}

		[Command("eightball", "8ball")]
		[Description("Ask the mysterious 8ball a question.")]
		public async Task EightballCommandAsync()
		{
			var response = _botFun.GetRandomEightballResponse();
			await Context.ReplyAsync(response);
		}
	}
}
