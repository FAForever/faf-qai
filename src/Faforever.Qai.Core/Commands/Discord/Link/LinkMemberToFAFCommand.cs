using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Discord.Link
{
	public class LinkMemberToFAFCommand : DiscordCommandModule
	{
		[Command("link")]
		[Description("Link your Discord account to your FAF account.")]
		public async Task LinkMemberToFafCommandAsync(DiscordCommandContext ctx)
		{

		}
	}
}
