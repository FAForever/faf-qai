using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Database;
using Faforever.Qai.Discord.Core.Structures.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Core.Services
{
	// Test Webhook URL
	// https://discord.com/api/webhooks/773214715328725012/Kj0P_CMUooloZTyfIOP37QD1GLuXp_LVRS3ax2FqxT5mP2dOvLoP_xMZiJ8L6Sf0jYgq

	public class RelayService : IDisposable
	{
		private readonly QAIDatabaseModel _database;
		private readonly ILogger _logger;
		private bool initalized;
		private bool disposedValue;

		private ConcurrentDictionary<string, HashSet<string>> IRCtoWebhookRelations { get; set; }

		public RelayService(QAIDatabaseModel database, ILogger logger)
		{
			this._database = database;
			this.initalized = false;
			this._logger = logger;
		}

		private bool Initalize()
		{
			try
			{
				IRCtoWebhookRelations = new ConcurrentDictionary<string, HashSet<string>>();

				var relays = _database.RelayConfigurations
					.AsNoTracking()
					.ToList();

				foreach(var r in relays)
				{
					foreach(var hook in r.Webhooks)
					{
						if(IRCtoWebhookRelations.ContainsKey(hook.Key))
						{
							IRCtoWebhookRelations[hook.Key].Add(hook.Value.WebhookUrl);
						}
						else
						{
							IRCtoWebhookRelations[hook.Key] = new HashSet<string>() { hook.Value.WebhookUrl };
						}
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

		public async Task<bool> AddRelayAsync(ulong discordGuild, ulong discordChannel, string ircChannel)
		{
			if (!this.initalized)
				if (!Initalize())
					throw new Exception("Failed to Initalize the RelayService.");



			return true;
		}

		public async Task<bool> RemoveRelayAsync(ulong discordGuild, ulong discordChannel)
		{
			if (!this.initalized)
				if (!Initalize())
					throw new Exception("Failed to Initalize the RelayService.");



			return true;
		}

		public async Task<bool> SendFromDiscordAsync(ulong discordChannel, string author, string message)
		{
			if (!this.initalized)
				if (!Initalize())
					throw new Exception("Failed to Initalize the RelayService.");



			return true;
		}

		public async Task<bool> SendFromIRCAsync(string ircChannel, string author, string message)
		{
			if (!this.initalized)
				if (!Initalize())
					throw new Exception("Failed to Initalize the RelayService.");



			return true;
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
				IRCtoWebhookRelations = null;
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
