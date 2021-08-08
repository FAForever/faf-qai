using System;

using DSharpPlus;

namespace Faforever.Qai.Core.Commands.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class RequireBotPermissionsAttribute : RequirePermissionsAttribute
    {
        public RequireBotPermissionsAttribute(Permissions discordPermissions) : base(discordPermissions) { }
        public RequireBotPermissionsAttribute(IrcPermissions ircPermissions) : base(ircPermissions) { }
        public RequireBotPermissionsAttribute(Permissions discordPermissions, IrcPermissions ircPermissions) : base(discordPermissions, ircPermissions) { }
    }
}
