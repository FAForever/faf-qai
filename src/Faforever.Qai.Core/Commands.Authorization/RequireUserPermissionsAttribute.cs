using System;
using System.Collections.Generic;
using System.Text;

using DSharpPlus;

using Faforever.Qai.Core.Commands.Authorization.Exceptions;

namespace Faforever.Qai.Core.Commands.Authorization
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class RequireUserPermissionsAttribute : Attribute, IPermissionsAttribute
	{
		public Permissions? DiscordPermissions { get; }
		public IrcPermissions? IRCPermissions { get; }

		public RequireUserPermissionsAttribute(Permissions discordPermissions)
		{
			DiscordPermissions = discordPermissions;
			IRCPermissions = null;
		}

		public RequireUserPermissionsAttribute(IrcPermissions ircPermissions)
		{
			DiscordPermissions = null;
			IRCPermissions = ircPermissions;
		}

		public RequireUserPermissionsAttribute(Permissions discordPermissions, IrcPermissions ircPermissions)
		{
			DiscordPermissions = discordPermissions;
			IRCPermissions = ircPermissions;
		}
	}
}
