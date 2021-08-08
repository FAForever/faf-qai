using System;

namespace Faforever.Qai.Core.Commands.Authorization
{
    [Flags]
    public enum IrcPermissions
    {
        None = 0,
        AeolusModerator = 1,
        ChannelModerator = 2,
    }
}
