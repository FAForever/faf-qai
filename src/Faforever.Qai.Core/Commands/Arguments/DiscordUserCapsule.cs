
using DSharpPlus.Entities;

namespace Faforever.Qai.Core.Commands.Arguments
{
    public class DiscordUserCapsule(DiscordUser u) : IBotUserCapsule
    {
        public string Username => User.Mention;
        public DiscordUser User { get; private set; } = u;
    }
}
