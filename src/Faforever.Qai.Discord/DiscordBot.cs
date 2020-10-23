using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;

using Faforever.Qai.Discord.Commands.Utils;
using Faforever.Qai.Discord.Structures.Configurations;
using Faforever.Qai.Discord.Utils.Bot;
using Faforever.Qai.Discord.Utils.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Discord
{
	public class DiscordBot
	{
		#region Event Ids
		// 127### - designates a Discord Bot event.
		public static EventId Event_CommandResponder { get; } = new EventId(127001, "Command Responder");
		public static EventId Event_EventResponder { get; } = new EventId(127002, "Event Responder");
		#endregion

		#region Public Variables
		/// <summary>
		/// The Discord Sharded Client of this Discord Bot.
		/// </summary>
		public DiscordShardedClient Client { get; private set; }
		/// <summary>
		/// The Rest client for the Discord Bot
		/// </summary>
		public DiscordRestClient Rest { get; private set; }
		/// <summary>
		/// The list of commands that the Bot will respond to.
		/// </summary>
		public IEnumerable<string> CommandList { get; private set; }
		#endregion
		#region Private Variables
		private DiscordBotConfiguration Config { get; set; }
		private EventResponder eventResponder;

		private readonly LogLevel logLevel;
		#endregion


		public DiscordBot(LogLevel logLevel = LogLevel.Debug)
		{
			this.logLevel = logLevel;
		}

		#region Confgiurations
		/// <summary>
		/// Register the DiscordBotConfiguration object for this bot.
		/// </summary>
		/// <returns>Task that registers the configuration</returns>
		public async Task RegisterBotConfigurationAsync()
		{
			// TODO: Bot Configuration reading. ENV variables vs JSON config vs .ini file?
		}
		#endregion

		#region Initialize
		/// <summary>
		/// Initalize the public variables with data and configure Bot Services.
		/// </summary>
		/// <returns>The Task of this operation</returns>
		public async Task InitializeAsync()
		{
			// Register necissary configurations
			await RegisterBotConfigurationAsync();

			// Create the Clients
			Client = new DiscordShardedClient(GetDiscordConfiguration());
			Rest = new DiscordRestClient(GetDiscordConfiguration());

			// create the Commands Next module
			var commands = await Client.UseCommandsNextAsync(GetCommandsNextConfiguration());

			foreach (CommandsNextExtension c in commands.Values)
			{
				// Register the command success and error handlers.
				c.CommandErrored += CommandResponder.RespondError;
				c.CommandExecuted += CommandResponder.RespondSuccess;

				// Get the assembly that contins the Discord Bot type, add thus all of its commands.
				c.RegisterCommands(Assembly.GetAssembly(typeof(DiscordBot)));

				// Set the help formatter
				c.SetHelpFormatter<HelpFormatter>();

				// And register the commands
				CommandList = c.RegisteredCommands.Keys;

				// Then register any converters that are needed
				// ex: c.RegisterConverter(new LeaderboardTypeConverter());
			}

			// Register any additional Client events
			eventResponder = new EventResponder(Client, Rest);
			eventResponder.Initalize();
		}
		/// <summary>
		/// Gets the DiscordConfiguration object for a DiscordBot.
		/// </summary>
		/// <returns>The DiscordConfiguration for a DiscordBot</returns>
		private DiscordConfiguration GetDiscordConfiguration()
		{
			var cfg = new DiscordConfiguration
			{
				Token = Config.Token,
				TokenType = TokenType.Bot,
				MinimumLogLevel = logLevel,
				ShardCount = Config.Shards, // Default to 1 for automatic sharding.
				Intents = DiscordIntents.GuildMessages,
			};

			return cfg;
		}
		/// <summary>
		/// Gets the CommandsNextConfiguration for the DiscrdBot
		/// </summary>
		/// <returns>A CommandsNextConfiguration for this DiscordBot</returns>
		private CommandsNextConfiguration GetCommandsNextConfiguration()
		{
			// Add bot services here!
			var services = new ServiceCollection();

			var ccfg = new CommandsNextConfiguration
			{
				EnableDms = false,
				EnableMentionPrefix = true,
				EnableDefaultHelp = true,
				CaseSensitive = false,
				IgnoreExtraArguments = true,
				//PrefixResolver = PrefixResolver, - This can be used for custom prefixes per server, commands to change prefixes
				// along with custom checking of messages before they are passed to the command handler.
				StringPrefixes = new string[] { Config.Prefix },
				Services = services.BuildServiceProvider(),
			};

			return ccfg;
		}
		#endregion

		#region Start
		/// <summary>
		/// Start the Discord Bot.
		/// </summary>
		/// <returns>The Task of this operation</returns>
		public async Task StartAsync()
		{
			// Start the Clients!
			await Client.StartAsync();
		}
		#endregion
	}
}
