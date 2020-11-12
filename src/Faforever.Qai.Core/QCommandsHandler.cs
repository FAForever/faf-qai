using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;

using Faforever.Qai.Core.Commands.Authorization;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Commands.Context.Exceptions;

using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;

using Qmmands;

namespace Faforever.Qai.Core
{
	public class QCommandsHandler
	{
		private readonly CommandService _commands;
		private readonly ILogger _logger;

		public QCommandsHandler(CommandService commands, ILogger<QCommandsHandler> logger)
		{
			this._commands = commands;
			this._logger = logger;
			_commands.CommandExecuted += Commands_CommandExecuted;
			_commands.CommandExecutionFailed += Commands_CommandExecutionFailed;
		}

		public async Task MessageRecivedAsync(CustomCommandContext baseContext, string message)
		{
			if (baseContext.Prefix is null || baseContext.Prefix == "")
				throw new NullPrefixException("Prefix cannont be null or blank.");

			if (!CommandUtilities.HasPrefix(message, baseContext.Prefix, out string output))
				return;

			//var res = await _commands.ExecuteAsync(output, baseContext);

			var cmds = _commands.FindCommands(output);

			var command = cmds.FirstOrDefault();

			if (command == default)
			{
				await baseContext.ReplyAsync("Command not found.");
				return;
			}

			var attributes = command.Command.Attributes;

			if(!await CheckPermissions(baseContext, attributes))
			{

			}

			var res = await command.Command.ExecuteAsync(output, baseContext);

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

		private async Task<bool> CheckPermissions(IRCCommandContext ctx, IReadOnlyList<Attribute> attributes)
		{


			return false;
		}

		private async Task<bool> CheckPermissions(DiscordCommandContext ctx, IReadOnlyList<Attribute> attributes)
		{
			Permissions userPerms = Permissions.None;
			Permissions botPerms = Permissions.None;

			foreach(var a in attributes)
			{
				if(a is IPermissionsAttribute perms)
				{
					if(!(perms.DiscordPermissions is null))
					{
						switch(perms)
						{
							case RequireUserPermissionsAttribute user:
								userPerms = (Permissions)(userPerms & perms.DiscordPermissions);
								break;

							case RequireBotPermissionsAttribute bot:
								botPerms = (Permissions)(botPerms & perms.DiscordPermissions);
								break;

							case RequirePermissionsAttribute both:
								userPerms = (Permissions)(userPerms & perms.DiscordPermissions);
								botPerms = (Permissions)(botPerms & perms.DiscordPermissions);
								break;
						}
					}
				}
			}

			bool userResult = true;
			bool botResult = true;

			if (userPerms != Permissions.None)
			{
				var member = await ctx.Guild.GetMemberAsync(ctx.Message.Author.Id);

				userResult = ctx.Channel.PermissionsFor(member).HasPermission(userPerms);
			}

			if(botPerms != Permissions.None)
			{
				var selfMember = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);

				botResult = ctx.Channel.PermissionsFor(selfMember).HasPermission(botPerms);
			}

			return userResult && botResult;
		}

		private async Task<bool> CheckPermissions(CustomCommandContext ctx, IReadOnlyList<Attribute> attributes)
		{
			var ircResult = false;
			var disResult = false;

			if (ctx is IRCCommandContext irc)
				ircResult = await CheckPermissions(irc, attributes);
			else if (ctx is DiscordCommandContext dis)
				disResult = await CheckPermissions(dis, attributes);

			return ircResult || disResult;
		}

		private async Task Commands_ParserFailed(DefaultArgumentParserResult? res, CustomCommandContext baseContext)
		{
			await baseContext.ReplyAsync($"Failed to parse an argument for command: {res?.Command.Name ?? "unkown"}\n{res?.Reason ?? ""}");
		}

		private Task Commands_CommandExecutionFailed(CommandExecutionFailedEventArgs e)
		{
			_logger.LogError(e.Result.Exception, $"Failed to execute command:\n{e.Result.Reason}");

			var ctx = e.Context as CustomCommandContext;

			if (!(ctx is null))
			{
				ctx.ReplyAsync($"Command execution failed: {e.Result.Reason}");
			}

			return Task.CompletedTask;
		}

		private Task Commands_CommandExecuted(CommandExecutedEventArgs e)
		{
			_logger.LogInformation($"Executed command: {e.Result?.Command.Name ?? e.Context?.Command.Name}");

			return Task.CompletedTask;
		}

		private Task Respond_ArgumentException()
		{


			return Task.CompletedTask;
		}
	}
}
