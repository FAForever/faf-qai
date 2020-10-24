using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Discord.Utils.Bot
{
	public class DiscordEventHandler
	{
		private readonly DiscordRestClient Rest;
		private readonly DiscordShardedClient Client;

		public DiscordEventHandler(DiscordShardedClient client, DiscordRestClient rest)
		{
			this.Client = client;
			this.Rest = rest;
		}

		public void Initalize()
		{
			// Register client events.
			Client.Ready += Client_Ready;
		}

		private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
		{
			Client.Logger.LogInformation(DiscordBot.Event_CommandHandler, "Client Ready!");

			return Task.CompletedTask;
		}
	}
}