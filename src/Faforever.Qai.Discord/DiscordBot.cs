using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.EventArgs;

using Faforever.Qai.Core;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Discord.Commands;
using Faforever.Qai.Discord.Core.Structures.Configurations;
using Faforever.Qai.Discord.Utils.Bot;
using Faforever.Qai.Discord.Utils.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Discord
{
	public class DiscordBot : IDisposable, IAsyncDisposable
	{
		#region Event Ids
		// 127### - designates a Discord Bot event.
		public static EventId Event_CommandResponder { get; } = new EventId(127001, "Command Responder");
		public static EventId Event_CommandHandler { get; } = new EventId(127002, "Command Handler");
		public static EventId Event_EventHandler { get; } = new EventId(127003, "Event Handler");
		#endregion

		#region Static Variables
		public static ConcurrentDictionary<CommandHandler, Tuple<Task, CancellationTokenSource>>? CommandsInProgress { get; private set; }
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
		public IReadOnlyDictionary<string, Command> Commands { get; private set; }
		#endregion

		#region Private Variables
		private DiscordBotConfiguration Config { get; set; }

		private readonly IServiceProvider _services;
		private readonly QCommandsHandler _commands;
		private readonly DiscordEventHandler _eventHandler;
		#endregion


		public DiscordBot(IServiceProvider services, DiscordBotConfiguration configuration,
			DiscordShardedClient client, DiscordRestClient rest,
			QCommandsHandler commands, DiscordEventHandler eventHandler)
		{
			CommandsInProgress = new ConcurrentDictionary<CommandHandler, Tuple<Task, CancellationTokenSource>>();
			Commands = new Dictionary<string, Command>();
			Config = configuration;
			Client = client;
			Rest = rest;
			this._services = services;
			this._commands = commands;
			this._eventHandler = eventHandler;
		}

		#region Confgiurations
		/// <summary>
		/// Register the DiscordBotConfiguration object for this bot.
		/// </summary>
		/// <returns>Task that registers the configuration</returns>
		public Task RegisterBotConfigurationAsync()
		{
			// TODO: Bot Configuration reading. ENV variables vs JSON config vs .ini file?

			return Task.CompletedTask;
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
			if (Config.Token == "")
				await RegisterBotConfigurationAsync();

			// Create the Clients

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
				Commands = c.RegisteredCommands;

				// Then register any converters that are needed
				c.RegisterConverter(new TimeSpanConverter());
			}

			// Register any additional Client events
			_eventHandler.Initalize();

			// Register the event needed to send data to the CommandHandler
			Client.MessageCreated += QMmands_MessageCreated;
		}

		/// <summary>
		/// Gets the CommandsNextConfiguration for the DiscrdBot
		/// </summary>
		/// <returns>A CommandsNextConfiguration for this DiscordBot</returns>
		private CommandsNextConfiguration GetCommandsNextConfiguration()
		{
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
				Services = _services,
				UseDefaultCommandHandler = false
			};

			return ccfg;
		}
		#endregion

		#region Command Events
		private Task QMmands_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
		{
			if (e.Author.IsBot) return Task.CompletedTask; // ignore bots.

			_ = Task.Run(async () =>
			{
				var ctx = new DiscordCommandContext(sender, e, Config.Prefix, _services);

				await _commands.MessageRecivedAsync(ctx, e.Message.Content);
			});

			return Task.CompletedTask;
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
			var relay = _services.GetRequiredService<RelayService>();
			await relay.InitializeAsync();
		}
		#endregion



		public void Dispose()
		{
			if (!(CommandsInProgress is null))
			{
				foreach (var cmd in CommandsInProgress.AsParallel())
				{
					cmd.Value.Item2.Cancel();
					cmd.Value.Item2.Dispose();
					cmd.Value.Item1.Dispose();
				}

				// Clear out the dict.
				CommandsInProgress = null;

				try
				{
					Client.StopAsync().GetAwaiter().GetResult();
				}
				catch (Exception ex)
				{
					Client.Logger.LogWarning(ex, "Failed to stop client.");
				}

				try
				{
					Rest.Dispose();
				}
				catch (Exception ex)
				{
					Client.Logger.LogWarning(ex, "Failed to displse the rest client.");
				}
			}
		}

		public async ValueTask DisposeAsync()
		{
			if (!(CommandsInProgress is null))
			{
				await Task.Run(() =>
				{
					foreach (var cmd in CommandsInProgress.AsParallel())
					{
						cmd.Value.Item2.Cancel();
						cmd.Value.Item2.Dispose();
						cmd.Value.Item1.Dispose();
					}
				});

				// Clear out the dict.
				CommandsInProgress = null;
				try
				{
					await Client.StopAsync();
				}
				catch (Exception ex)
				{
					Client.Logger.LogWarning(ex, "Failed to stop client.");
				}

				try
				{
					Rest.Dispose();
				}
				catch (Exception ex)
				{
					Client.Logger.LogWarning(ex, "Failed to displse the rest client.");
				}
			}
		}
	}
}
