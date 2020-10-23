using System.Threading.Tasks;

using DSharpPlus;

using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Discord.Utils.Bot
{
	public class EventResponder
	{
		private readonly DiscordRestClient Rest;
		private readonly DiscordShardedClient Client;

		public EventResponder(DiscordShardedClient client, DiscordRestClient rest)
		{
			this.Client = client;
			this.Rest = rest;
		}

		public void Initalize()
		{
			// Register client events.
			Client.Ready += Client_Ready;
		}

		private Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
		{
			Client.Logger.LogInformation(DiscordBot.Event_EventResponder, "Client Ready!");

			return Task.CompletedTask;
		}
	}
}