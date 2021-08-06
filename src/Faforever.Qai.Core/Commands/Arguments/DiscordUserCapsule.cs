
using DSharpPlus.Entities;

namespace Faforever.Qai.Core.Commands.Arguments
{
	public class DiscordUserCapsule : IBotUserCapsule
	{
		public string Username => User.Mention;
		public DiscordUser User { get; private set; }
		public DiscordUserCapsule(DiscordUser u)
		{
			User = u;
		}
	}
}
