using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace Faforever.Qai.Core.Commands.Arguments
{
	public class DiscordMemberCapsule : IBotUserCapsule
	{
		public string Username => Member.Mention;

		public DiscordMember Member { get; private set; }
		public DiscordMemberCapsule(DiscordMember m)
		{
			Member = m;
		}
	}
}
