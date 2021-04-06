using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Services;

using Microsoft.Extensions.Configuration;

using Qmmands;

using static Faforever.Qai.Core.Services.AccountLinkService;

namespace Faforever.Qai.Core.Commands.Discord.Link
{
	public class LinkMemberToFAFCommand : DiscordCommandModule
	{
		private readonly AccountLinkService _link;
		private readonly IConfiguration _configuration;

		public LinkMemberToFAFCommand(AccountLinkService link,
			IConfiguration configuration)
		{
			_link = link;
			_configuration = configuration;
		}

		[Command("link")]
		[Description("Link your Discord account to your FAF account.")]
		public async Task LinkMemberToFafCommandAsync()
		{
			try
			{
				var token = await _link.StartAsync(Context.Guild.Id, Context.User.Id, Context.User.Username);

				var member = await Context.Guild.GetMemberAsync(Context.User.Id);

				await member.SendMessageAsync($"https://{_configuration["Config:Host"]}/api/link/token/{HttpUtility.HtmlEncode(token)}");
			}
			catch (DiscordIdAlreadyLinkedException)
			{
				await Context.ReplyAsync("This Discord account has already been linked.");
			}
		}
	}
}
