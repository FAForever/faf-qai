
using IrcDotNet;

namespace Faforever.Qai.Core.Commands.Arguments
{
    public class IrcUserCapsule(IrcUser u) : IBotUserCapsule
    {
        public string Username => User.NickName;
        public IrcUser User { get; private set; } = u;
    }
}
