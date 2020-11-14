using System;
using System.Collections.Generic;
using System.Text;

using DSharpPlus;

namespace Faforever.Qai.Core.Commands.Authorization
{
	public interface IPermissionsAttribute
	{
		public Permissions? DiscordPermissions { get; }
		public string? IRCPermissions { get; }
	}
}
