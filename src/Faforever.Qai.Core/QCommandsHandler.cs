using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using Faforever.Qai.Core.Commands;
using Faforever.Qai.Core.Commands.Authorization;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Commands.Context.Exceptions;
using IrcDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Qmmands;

namespace Faforever.Qai.Core
{
    public class QCommandsHandler
    {
        public static readonly Random Rand = new Random(); // for basic random operations

        private readonly CommandService _commands;
        private readonly ILogger _logger;
        private readonly HashSet<ulong> _fafStaff;

        public QCommandsHandler(CommandService commands, ILogger<QCommandsHandler> logger, IConfiguration config)
        {
            this._commands = commands;
            this._logger = logger;
            this._fafStaff = new(from child in config.GetSection("Roles:FafStaff").GetChildren()
                                 where ulong.TryParse(child.Value, out _)
                                 select ulong.Parse(child.Value));
            _commands.CommandExecuted += Commands_CommandExecuted;
            _commands.CommandExecutionFailed += Commands_CommandExecutionFailed;
        }

        public async Task MessageRecivedAsync(CustomCommandContext baseContext, string message)
        {
            if (baseContext.Prefix is null || baseContext.Prefix == "")
                throw new NullPrefixException("Prefix cannont be null or blank.");

            if (message[0] == '#')
            {
                var rest = message[1..];
                if (!string.IsNullOrEmpty(rest) && rest.All(char.IsDigit))
                    message = $"{baseContext.Prefix}replay {rest}";
            }

            if (!CommandUtilities.HasPrefix(message, baseContext.Prefix, out string output))
                return;

            //var res = await _commands.ExecuteAsync(output, baseContext);

            var cmds = _commands.FindCommands(output);

            var command = cmds.FirstOrDefault(match => {
                var modType = match.Command.Module.Type;
                if (modType.IsAssignableTo(typeof(DualCommandModule)))
                    return true;

                var inDiscord = baseContext is DiscordCommandContext;
                var discordCommand = modType.IsAssignableTo(typeof(DiscordCommandModule));

                return inDiscord == discordCommand;
            });

            if (command == default)
            {
                _logger.LogDebug($"Command {message} not found!");
                //await baseContext.ReplyAsync("Command not found.");
                return;
            }

            var required = GetCommandRequirements(command.Command);
            var success = await baseContext.CheckPermissionsAsync(required);
            if (!success)
                return;

            var parts = output.Split(" ");
            var arguments = parts.Length > 0 ? string.Join(" ", parts[1..]) : "";

            var res = await command.Command.ExecuteAsync(arguments, baseContext);

            if (res is null || !res.IsSuccessful)
            {
                switch (res)
                {
                    case DefaultArgumentParserResult argRes:
                        await Commands_ParserFailed(argRes, baseContext);
                        break;
                }
            }
        }

        private static CommandRequirements GetCommandRequirements(Command command)
        {
            var permissionAttributes = command.Attributes.Where(c => c is IPermissionsAttribute).Cast<IPermissionsAttribute>();

            static void AddPermissions(CommandRequirements requirements, IPermissionsAttribute attribute)
            {
                bool bot = false, user = false;
                var discord = attribute.DiscordPermissions ?? Permissions.None;
                var irc = attribute.IRCPermissions ?? IrcPermissions.None;

                switch (attribute)
                {
                    case RequireUserPermissionsAttribute:
                        user = true;
                        break;
                    case RequireBotPermissionsAttribute:
                        bot = true;
                        break;
                    case RequirePermissionsAttribute:
                        bot = true;
                        user = true;
                        break;
                    case RequireFafStaffAttribute:
                        requirements.Discord.FafStaff = true;
                        break;
                }

                if (bot)
                {
                    requirements.Discord.Bot |= discord;
                    requirements.Irc.Bot |= irc;
                }

                if (user)
                {
                    requirements.Discord.User |= discord;
                    requirements.Irc.User |= irc;
                }
            }
            
            var req = new CommandRequirements();
            foreach (var attribute in permissionAttributes)
                AddPermissions(req, attribute);

            return req;
        }

        private static async Task Commands_ParserFailed(DefaultArgumentParserResult? res, CustomCommandContext baseContext)
        {
            await baseContext.ReplyAsync($"Failed to parse an argument for command: {res?.Command.Name ?? "unkown"}\n{res?.FailureReason ?? ""}");
        }

        private Task Commands_CommandExecutionFailed(CommandExecutionFailedEventArgs e)
        {
            _logger.LogError(e.Result.Exception, $"Failed to execute command:\n{e.Result.FailureReason}");


            if (e.Context is CustomCommandContext ctx)
                ctx.ReplyAsync($"Command execution failed: {e.Result.FailureReason}", ReplyOption.InPrivate);

            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutedEventArgs e)
        {
            _logger.LogInformation($"Executed command: {e.Result?.Command.Name ?? e.Context?.Command.Name}");

            return Task.CompletedTask;
        }

        private static Task Respond_ArgumentException()
        {
            return Task.CompletedTask;
        }
    }
}
