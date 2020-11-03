using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Structures;
using Faforever.Qai.Core.Structures.Configurations;
using Faforever.Qai.Discord.Core.Structures.Configurations;
using Faforever.Qai.Discord.Utils.Commands;

using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Discord.Utils.Bot
{
	public class CommandHandler
	{
		private readonly IReadOnlyDictionary<string, Command> _commands;
		private readonly DiscordBotConfiguration _config;
		private readonly DiscordClient _client;
		private readonly ILogger<BaseDiscordClient> _logger;

		public CommandHandler(IReadOnlyDictionary<string, Command> commands, DiscordClient client, DiscordBotConfiguration botConfig)
		{
			this._commands = commands;
			this._config = botConfig;
			this._client = client;
			this._logger = this._client.Logger;
		}

		// TODO: Update to save guild config state. This will run as is, but will not hold any saved data between sessions.
		public async Task MessageReceivedAsync(CommandsNextExtension cnext, DiscordMessage msg, CancellationToken cancellationToken = new CancellationToken())
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();

				using QAIDatabaseModel model = new QAIDatabaseModel();
				// Need to know how we are accessing the database!

				DiscordGuildConfiguration guildConfig = await model.FindAsync<DiscordGuildConfiguration>(msg.Channel.GuildId);

				if (guildConfig is null)
				{
					guildConfig = new DiscordGuildConfiguration
					{
						GuildId = msg.Channel.GuildId,
						Prefix = _config.Prefix
					};

					model.Add(guildConfig);

					await model.SaveChangesAsync();
				}

				cancellationToken.ThrowIfCancellationRequested();

				int prefixPos = await PrefixResolver(msg, guildConfig);

				if (prefixPos == -1)
					return; // Prefix is wrong, dont respond to this message.

				var prefix = msg.Content.Substring(0, prefixPos);
				string commandString = msg.Content.Replace(prefix, string.Empty);

				var command = cnext.FindCommand(commandString, out string args);

				cancellationToken.ThrowIfCancellationRequested();

				if (command is null)
				{ // Looks like that command does not exsist!
					await CommandResponder.RespondCommandNotFound(msg.Channel, prefix);
				}
				else
				{ // We found a command, lets deal with it.
					var ctx = cnext.CreateContext(msg, prefix, command, args);
					// We are done here, its up to CommandsNext now.

					cancellationToken.ThrowIfCancellationRequested();

					await cnext.ExecuteCommandAsync(ctx);
				}
			}
			finally
			{
				if (!(DiscordBot.CommandsInProgress is null))
				{
					if (DiscordBot.CommandsInProgress.TryRemove(this, out var taskData))
					{
						taskData.Item2.Dispose();
						taskData.Item1.Dispose();
					}
				}
			}
		}

		public async Task<int> PrefixResolver(DiscordMessage msg, DiscordGuildConfiguration guildConfig)
		{
			if (!msg.Channel.PermissionsFor(await msg.Channel.Guild.GetMemberAsync(_client.CurrentUser.Id).ConfigureAwait(false)).HasPermission(Permissions.SendMessages)) return -1; //Checks if bot can't send messages, if so ignore.
			else if (msg.Content.StartsWith(_client.CurrentUser.Mention)) return _client.CurrentUser.Mention.Length; // Always respond to a mention.
			else
			{
				try
				{
					foreach (string cmd in _commands.Keys) //Loop through all current commands.
					{
						if (msg.Content.StartsWith(guildConfig.Prefix + cmd)) //Check if message starts with prefix AND command.
						{
							return guildConfig.Prefix.Length; //Return length of server prefix.
						}
					}

					return -1; //If not, ignore.
				}
				catch (Exception err)
				{
					_logger.LogError(DiscordBot.Event_CommandHandler, $"Prefix Resolver failed in guild {msg.Channel.Guild.Name}:", DateTime.Now, err);
					return -1;
				}
			}
		}
	}
}