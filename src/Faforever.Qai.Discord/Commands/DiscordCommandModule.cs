using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Discord.Commands
{
	public class DiscordCommandModule : ModuleBase<DiscordCommandContext>
	{
		[Command("discordtest")]
		public async Task DOQTestAsync() 
			=> await DiscordOnlyQmmandsTestCommand.Execute(Context);
	}
}
