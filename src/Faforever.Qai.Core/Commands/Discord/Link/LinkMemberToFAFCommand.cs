using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Database.Entities;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Structures.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Qmmands;

using static Faforever.Qai.Core.Services.AccountLinkService;

namespace Faforever.Qai.Core.Commands.Discord.Link
{
	public class LinkMemberToFAFCommand : DiscordCommandModule
	{
		private readonly AccountLinkService _link;
		private readonly IConfiguration _configuration;
		private readonly IServiceProvider _services;

		public LinkMemberToFAFCommand(AccountLinkService link,
			IConfiguration configuration, IServiceProvider services)
		{
			_link = link;
			_configuration = configuration;
			_services = services;
		}

		[Command("link")]
		[Description("Link your Discord account to your FAF account. If you are already linked, use this command to get your role.")]
		public async Task LinkMemberToFafCommandAsync()
		{
			var member = await Context.Guild.GetMemberAsync(Context.User.Id);

			try
			{
				var token = await _link.StartAsync(Context.Guild.Id, Context.User.Id, Context.User.Username);

				await member.SendMessageAsync($"https://{_configuration["Config:Host"]}/api/link/token/{HttpUtility.HtmlEncode(token)}");
			}
			catch (DiscordIdAlreadyLinkedException)
			{
				var db = _services.GetRequiredService<QAIDatabaseModel>();
				var guild = await db.FindAsync<DiscordGuildConfiguration>(Context.Guild.Id);

				if(guild is not null
					&& guild.RoleWhenLinked is not null)
				{

					if (member.Roles.Any(x => x.Id == guild.RoleWhenLinked.Value))
					{
						await Context.ReplyAsync("This Discord account has already been linked, and you already have the assigned role for this server.");
					}
					else
					{
						var role = Context.Guild.GetRole(guild.RoleWhenLinked.Value);
						await member.GrantRoleAsync(role);
						await Context.ReplyAsync($"The role: {role.Name} has been given to your linked account.");
					}
				}
				else
				{
					await Context.ReplyAsync("This Discord account has already been linked.");
				}
			}
		}
	}
}
