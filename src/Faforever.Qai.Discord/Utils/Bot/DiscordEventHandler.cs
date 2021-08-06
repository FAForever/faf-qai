using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Database.Entities;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Structures.Configurations;
using Faforever.Qai.Core.Structures.Link;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Discord.Utils.Bot
{
	public class DiscordEventHandler
	{
		private readonly DiscordRestClient Rest;
		private readonly DiscordShardedClient Client;
		private readonly RelayService _relay;
		private readonly AccountLinkService _linkService;
		private readonly IServiceProvider _services;
		private readonly ILogger _logger;

		public DiscordEventHandler(DiscordShardedClient client, DiscordRestClient rest, 
			RelayService relay, AccountLinkService linkService, IServiceProvider services)
		{
			this.Client = client;
			this.Rest = rest;

			this._relay = relay;
			this._logger = Client.Logger;
			this._linkService = linkService;
			this._services = services;
		}

		public void Initalize()
		{
			// Register client events.
			Client.Ready += Client_Ready;

			#region Relay
			Client.MessageCreated += Relay_MessageReceived;
			#endregion

			#region Account Links
			_linkService.LinkComplete += _linkService_LinkComplete;
			#endregion
		}

		private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
		{
			Client.Logger.LogInformation(DiscordBot.Event_CommandHandler, "Client Ready!");

			return Task.CompletedTask;
		}

		#region Relay
		private Task Relay_MessageReceived(DiscordClient sender, MessageCreateEventArgs e)
		{
			if (e.Author.IsBot) return Task.CompletedTask;

			_ = Task.Run(async () => await _relay.Discord_MessageReceived(e.Channel.Id, e.Author.Username, e.Message.Content));

			return Task.CompletedTask;
		}
		#endregion

		#region Account Linking
		private Task _linkService_LinkComplete(LinkCompleteEventArgs args)
		{
			_ = Task.Run(async () => await AssignLinkedRole(args));

			return Task.CompletedTask;
		}

		private async Task AssignLinkedRole(LinkCompleteEventArgs args)
		{
			var db = _services.GetRequiredService<QAIDatabaseModel>();

			var guild = await db.FindAsync<DiscordGuildConfiguration>(args.Guild);

			if(guild is not null
				&& guild.RoleWhenLinked is not null
				&& args.Complete)
			{
				try
				{
					await Rest.AddGuildMemberRoleAsync(guild.GuildId, args.Link?.DiscordId ?? 0, guild.RoleWhenLinked.Value, "Account Linked.");
				}
				catch (Exception ex)
				{
					_logger.LogWarning(ex, "An error occoured when attempting to assign the linked role.");
				}
			}
		}
		#endregion
	}
}