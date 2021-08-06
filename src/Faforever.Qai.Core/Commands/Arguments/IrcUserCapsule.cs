
using IrcDotNet;

namespace Faforever.Qai.Core.Commands.Arguments
{
	public class IrcUserCapsule : IBotUserCapsule
	{
		public string Username => User.NickName;
		public IrcUser User { get; private set; }

		public IrcUserCapsule(IrcUser u)
		{
			User = u;
		}
	}
}
