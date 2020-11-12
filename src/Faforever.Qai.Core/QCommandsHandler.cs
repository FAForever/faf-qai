using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Commands.Context.Exceptions;

using Microsoft.Extensions.Logging;

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


			var res = await _commands.ExecuteAsync(output, baseContext);

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
