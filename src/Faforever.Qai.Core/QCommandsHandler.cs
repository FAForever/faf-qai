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
		public static readonly Random Rand = new Random(); // for basic random operations

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
				await baseContext.ReplyAsync("You do not have permissions to run that command!");
				return;
			}

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

		private async Task<bool> CheckPermissions(IRCCommandContext ctx, IReadOnlyList<Attribute> attributes)
		{
			// TODO: See if we need more complicated permission checking for the IRC client
			// If the authro is an operator, then they can run the command ...
			if (ctx.Author.IsOperator) return true;
			// ... otherwise, for every attribute ...
			foreach(var a in attributes)
			{// ... if it is a permissions attribute ...
				if(a is IPermissionsAttribute perms)
				{ // ... and it has IRC permissions ...
					if(perms.IRCPermissions is not null)
					{ // ... if it is requireing an operator ...
						if (perms.IRCPermissions.Value == IrcPermissions.Operator)
							return false; // ... then this user cant run the command ...
					}
				}
			}
			// ... otherwise they can run the command.
			return true;
		}

		private async Task<bool> CheckPermissions(DiscordCommandContext ctx, IReadOnlyList<Attribute> attributes)
		{
			// Create the inital permissions needed to run the command.
			// Set them to None, or no perms needed.
			uint userPerms = 0x0;
			uint botPerms = 0x0;

			// For every attribute on the command ...
			foreach(var a in attributes)
			{
				// ... if that attribute is a permissions attribute ...
				if(a is IPermissionsAttribute perms)
				{
					// ... and it has a valid Discord permission ...
					if(!(perms.DiscordPermissions is null))
					{
						// ... see what type of permission attribute it is ...
						switch(perms)
						{
							// ... if it is a user permissions ...
							case RequireUserPermissionsAttribute user:
								// ... Add the requierment to the userPerms.
								userPerms |= (uint)perms.DiscordPermissions;
								break;
							// ... if it is a bot permission ...
							case RequireBotPermissionsAttribute bot:
								// ... add the requirement to the botPerms.
								botPerms |= (uint)perms.DiscordPermissions;
								break;
							// ... if it is for both ...
							case RequirePermissionsAttribute both:
								// ... add the requirement to both.
								userPerms |= (uint)perms.DiscordPermissions;
								botPerms |= (uint)perms.DiscordPermissions;
								break;
						}
					}
				}
			}

			// Initalize the result variables. Assume the check will pass.
			bool userResult = true;
			bool botResult = true;
			// If there is no requirement for user permissions, skip this.
			if (userPerms != 0x0)
			{
				// Get the member object for the Author DiscordUser ...
				var member = await ctx.Guild.GetMemberAsync(ctx.Message.Author.Id);
				// ... and check that they have requiered permissions in the channel the command was from ...
				var perm = (Permissions)userPerms;
				userResult = ctx.Channel.PermissionsFor(member).HasPermission(perm);
			}
			// If there is no requirement for bot permissions, skip this.
			if(botPerms != 0x0)
			{
				// Get the bots member object for the server the command was in ....
				var selfMember = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
				// ... and check that it has the required permissions in the channel the command was from ...
				var perm = (Permissions)botPerms;
				botResult = ctx.Channel.PermissionsFor(selfMember).HasPermission(perm);
			}
			// return true if both check pass, otherwise false.
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
