using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Structures.Configurations;

using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Discord.Utils.Bot
{
	public class DiscordEventHandler
	{
		private readonly DiscordRestClient Rest;
		private readonly DiscordShardedClient Client;
		private readonly RelayService _relay;
		private readonly ILogger _logger;

		private ConcurrentDictionary<MessageCreateEventArgs, Task> Relays { get; set; } = new ConcurrentDictionary<MessageCreateEventArgs, Task>();

		public DiscordEventHandler(DiscordShardedClient client, DiscordRestClient rest, RelayService relay)
		{
			this.Client = client;
			this.Rest = rest;

			this._relay = relay;
			this._logger = Client.Logger;
		}

		public void Initalize()
		{
			// Register client events.
			Client.Ready += Client_Ready;

			#region Relay
			Client.MessageCreated += Relay_MessageReceived;
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
			Relays[e] = Task.Run(async () =>
			{
				await _relay.SendFromDiscordAsync(e.Channel.Id, e.Author.Username, e.Message.Content);

				// Remove this task from the stored list.
				Relays.TryRemove(e, out _);
			});

			return Task.CompletedTask;
		}
		#endregion
	}
}