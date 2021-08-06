using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;

using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Database.Entities;
using Faforever.Qai.Core.Structures.Webhooks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Core.Services
{
	// Test Webhook URL
	// https://discord.com/api/webhooks/773214715328725012/Kj0P_CMUooloZTyfIOP37QD1GLuXp_LVRS3ax2FqxT5mP2dOvLoP_xMZiJ8L6Sf0jYgq

	public class RelayService : IDisposable
	{
		public delegate Task RelayDiscordMessage(string ircChannel, string author, string message);
		public event RelayDiscordMessage? DiscordMessageReceived;

		private readonly IServiceProvider _services;
		private readonly ILogger _logger;
		private readonly DiscordRestClient _rest;
		private bool initalized;
		private bool disposedValue;

		private ConcurrentDictionary<string, HashSet<(ulong, string)>> IRCToWebhookRelations { get; set; }
		private ConcurrentDictionary<ulong, string> DiscordToIRCWebhookRelations { get; set; }

		private HttpClient Http { get; set; }

		public RelayService(IServiceProvider services, ILogger<RelayService> logger,
			DiscordRestClient rest)
		{
			this._services = services;
			this.initalized = false;
			this._logger = logger;
			this._rest = rest;

			Http = new HttpClient();
			IRCToWebhookRelations = new();
			DiscordToIRCWebhookRelations = new();
		}

		public async Task<bool> InitalizeAsync()
		{
			if (initalized)
				return true;

			try
			{
				var _database = _services.GetRequiredService<QAIDatabaseModel>();

				var relays = _database.RelayConfigurations
					.AsNoTracking()
					.ToList();

				foreach (var r in relays)
				{
					foreach (var hook in r.Webhooks)
					{
						AddToWebhookDict(hook.Key, hook.Value.Id, hook.Value.Token);
					}

					foreach(var links in r.DiscordToIRCLinks)
					{
						var live = await _rest.GetWebhookAsync(links.Key);
						DiscordToIRCWebhookRelations[live.ChannelId] = links.Value;
					}
				}

				this.initalized = true;
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Relay Service failed to initalize.");
				this.initalized = false;
			}

			return this.initalized;
		}

		private void AddToWebhookDict(string key, ulong id, string token)
		{
			if (IRCToWebhookRelations.ContainsKey(key))
			{
				IRCToWebhookRelations[key].Add((id, token));
			}
			else
			{
				IRCToWebhookRelations[key] = new HashSet<(ulong, string)>() { (id, token) };
			}
		}

		public async Task<bool> AddRelayAsync(ulong discordGuild, DiscordWebhook hook, string ircChannel)
		{
			try
			{
				if (!this.initalized)
					if (!await InitalizeAsync())
						throw new Exception("Failed to Initalize the RelayService.");

				var _database = _services.GetRequiredService<QAIDatabaseModel>();

				AddToWebhookDict(ircChannel, hook.Id, hook.Token);

				DiscordToIRCWebhookRelations[hook.ChannelId] = ircChannel;

				// This method should not be passed values that dont have a configuration value created for them.
				var cfg = await _database.FindAsync<RelayConfiguration>(discordGuild);
				if (cfg is null)
					throw new Exception("Failed to get valid relay configuration.");

				_database.Update(cfg);

				var hookData = new DiscordWebhookData()
				{
					Id = hook.Id,
					Token = hook.Token
				};

				cfg.Webhooks[ircChannel] = hookData;
				cfg.DiscordToIRCLinks[hook.Id] = ircChannel;

				await _database.SaveChangesAsync();

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to add new Relay.");
				return false;
			}
		}

		public async Task<bool> RemoveRelayAsync(ulong discordGuild, ulong webhookId)
		{
			try
			{
				if (!this.initalized)
					if (!await InitalizeAsync())
						throw new Exception("Failed to Initalize the RelayService.");


				var _database = _services.GetRequiredService<QAIDatabaseModel>();

				// This method should not be passed values that dont have a configuration vlaue created for them.
				var cfg = await _database.FindAsync<RelayConfiguration>(discordGuild);
				if (cfg is null)
					throw new Exception("Failed to get valid relay configuration.");
				DiscordWebhookData? hook = null;
				if (cfg.DiscordToIRCLinks.TryRemove(webhookId, out string? ircChannel))
				{
					// At least one thing changed, so tell the database to save changes.
					_database.Update(cfg);

					if (cfg.Webhooks.TryRemove(ircChannel, out hook))
					{
						IRCToWebhookRelations[ircChannel]?.Remove((hook.Id, hook.Token));

						var realHook = await _rest.GetWebhookWithTokenAsync(hook.Id, hook.Token);

						_ = DiscordToIRCWebhookRelations.TryRemove(realHook.ChannelId, out _);

						await realHook.DeleteAsync();
					}
				}

				await _database.SaveChangesAsync();

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to remove Relay.");
				return false;
			}
		}

		public async Task Discord_MessageReceived(ulong discordChannel, string author, string message)
		{
			if (!this.initalized)
				if (!await InitalizeAsync()) return; // ignore

			if(DiscordToIRCWebhookRelations.TryGetValue(discordChannel, out var ircChannel))
			{
				try
				{
					// send to IRC
					if (DiscordMessageReceived != null)
						await DiscordMessageReceived(ircChannel, author, message);

					// bounce to other discord channels listening
					await IRC_MessageReceived(ircChannel, author, message, discordChannel);
				}
				catch
				{
					_logger.LogWarning("No IRC receivers registered.");
				}
			}
		}

		public async Task IRC_MessageReceived(string ircChannel, string author, string message, ulong channelToIgnore = 0)
		{
			if (!this.initalized)
				if (!await InitalizeAsync()) return; // ignore

			if (IRCToWebhookRelations.TryGetValue(ircChannel, out var hooks))
			{
				foreach (var h in hooks)
				{
					var msg = new DiscordWebhookBuilder()
							.WithUsername(author)
							.WithContent(message);

					if (channelToIgnore != 0)
					{
						var hook = await _rest.GetWebhookWithTokenAsync(h.Item1, h.Item2);

						if (hook.ChannelId == channelToIgnore)
							continue;

						await hook.ExecuteAsync(msg);
					}
					else
					{
						await _rest.ExecuteWebhookAsync(h.Item1, h.Item2, msg);
					}
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				IRCToWebhookRelations.Clear();
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~RelayService()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
