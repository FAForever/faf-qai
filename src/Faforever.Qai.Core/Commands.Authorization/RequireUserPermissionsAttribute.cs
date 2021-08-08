using System;

using DSharpPlus;

namespace Faforever.Qai.Core.Commands.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class RequireUserPermissionsAttribute : RequirePermissionsAttribute
    {
        public RequireUserPermissionsAttribute(Permissions discordPermissions) : base(discordPermissions) { }
        public RequireUserPermissionsAttribute(IrcPermissions ircPermissions) : base(ircPermissions) { }
        public RequireUserPermissionsAttribute(Permissions discordPermissions, IrcPermissions ircPermissions) : base(discordPermissions, ircPermissions) { }
    }
}
