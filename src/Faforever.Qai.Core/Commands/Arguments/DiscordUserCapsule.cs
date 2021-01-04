using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace Faforever.Qai.Core.Commands.Arguments
{
	public class DiscordUserCapsule : IBotUserCapsule
	{
		public DiscordUser User { get; private set; }
		public DiscordUserCapsule(DiscordUser u)
		{
			User = u;
		}
	}
}
