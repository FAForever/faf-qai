using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Irc.Commands
{
	public class IrcCommandModule : ModuleBase<IRCCommandContext>
	{
		[Command("irctest")]
		public async Task TestIrcCommand()
			=> await IrcOnlyTestCommand.Execute(Context);
	}
}
