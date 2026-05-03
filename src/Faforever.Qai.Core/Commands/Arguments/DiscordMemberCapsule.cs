
using DSharpPlus.Entities;

namespace Faforever.Qai.Core.Commands.Arguments
{
    public class DiscordMemberCapsule(DiscordMember m) : IBotUserCapsule
    {
        public string Username => Member.Mention;

        public DiscordMember Member { get; private set; } = m;
    }
}
