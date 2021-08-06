using System;

namespace Faforever.Qai.Core.Commands.Discord.Role
{
	public partial class RoleCommands : DiscordCommandModule
	{
		private readonly IServiceProvider _services;

		public RoleCommands(IServiceProvider services)
		{
			_services = services;
		}
	}
}
