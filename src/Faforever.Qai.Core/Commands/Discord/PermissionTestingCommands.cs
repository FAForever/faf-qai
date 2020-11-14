using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Authorization;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Discord
{
	public class PermissionTestingCommands : DiscordCommandModule
	{
		[Command("testbotperms")]
		[RequireBotPermissions(DSharpPlus.Permissions.ManageMessages)]
		public async Task BotPermsCheck()
		{
			await RespondBasicSuccess("Valid!");
		}

		[Command("testuserperms")]
		[RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
		public async Task UserPremsCheck()
		{
			await RespondBasicSuccess("Valid!");
		}

		[Command("testbothperms")]
		[RequirePermissions(DSharpPlus.Permissions.Administrator)]
		public async Task BothPermsCheck()
		{
			await RespondBasicSuccess("Valid!");
		}
	}
}
