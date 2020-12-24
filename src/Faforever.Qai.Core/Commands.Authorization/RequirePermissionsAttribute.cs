using System;
using System.Collections.Generic;
using System.Text;

using DSharpPlus;

namespace Faforever.Qai.Core.Commands.Authorization
{
	[System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class RequirePermissionsAttribute : Attribute, IPermissionsAttribute
	{
		public Permissions? DiscordPermissions { get; }
		public IrcPermissions? IRCPermissions { get; }

		public RequirePermissionsAttribute(Permissions discordPermissions)
		{
			DiscordPermissions = discordPermissions;
			IRCPermissions = null;
		}

		public RequirePermissionsAttribute(IrcPermissions ircPermissions)
		{
			DiscordPermissions = null;
			IRCPermissions = ircPermissions;
		}

		public RequirePermissionsAttribute(Permissions discordPermissions, IrcPermissions ircPermissions)
		{
			DiscordPermissions = discordPermissions;
			IRCPermissions = ircPermissions;
		}
	}
}
