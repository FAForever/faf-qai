using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Arguments;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Commands.Exceptions;
using Faforever.Qai.Core.Services.BotFun;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Fun
{
	public class TauntCommand : DualCommandModule
	{
		private readonly IBotFunService _botFun;

		public TauntCommand(IBotFunService botFun)
		{
			_botFun = botFun;
		}

		[Command("taunt")]
		[Description("Taunt a user")]
		public async Task TauntCommandAsync(IBotUserCapsule? user = null)
		{
			string prefix = "";
			if(user is DiscordMemberCapsule or DiscordUserCapsule)
			{
				if (user is DiscordMemberCapsule member)
					prefix = member.Member.Mention;
				else if (user is DiscordUserCapsule userCapsule)
					prefix = userCapsule.User.Mention;
			}
			else if(user is IrcUserCapsule ircUser)
			{
				prefix = ircUser.User.UserName;
			}
			else if(user is null)
			{
				if (Context is DiscordCommandContext dctx)
					prefix = dctx.Client.CurrentUser.Mention;
				else if (Context is IRCCommandContext ictx)
					prefix = ictx.Client.UserName;
			}

			await Context.ReplyAsync($"{prefix}: {_botFun.GetRandomTaunt()}");
		}
	}
}
