using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;

using Faforever.Qai.Discord.Commands;

using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Discord.Utils.Commands
{
	public static class CommandResponder
	{
		public static Task RespondSuccess(CommandsNextExtension cnext, CommandExecutionEventArgs e)
		{
			// let's log the name of the command and user
			e.Context.Client.Logger.LogInformation(DiscordBot.Event_CommandResponder, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

			return Task.CompletedTask;
		}

		public static async Task RespondError(CommandsNextExtension cnext, CommandErrorEventArgs e)
		{
			if (e == null) return;
			if (e.Exception is ChecksFailedException cfex)
			{
				await ChecksFailedResponderAsync(e, cfex).ConfigureAwait(false);
			}
			else if (e.Exception is ArgumentException)
			{
				await ArgumentResponder(e).ConfigureAwait(false);
			}
			else
			{
				var embed = CommandModule.ErrorBase()
					.WithDescription($"An unhadled error occoured: {e.Exception.Message}");

				await e.Context.RespondAsync(embed: embed).ConfigureAwait(false);
			}
		}

		private static async Task ChecksFailedResponderAsync(CommandErrorEventArgs args, ChecksFailedException e)
		{
			var embed = CommandModule.ErrorBase()
				.WithDescription($"Invalid Permissions: {e.Message}");

			await args.Context.RespondAsync(embed: embed);
		}

		private static async Task ArgumentResponder(CommandErrorEventArgs args)
		{
			var embed = CommandModule.ErrorBase()
				.WithDescription($"Invalid Arguments");

			await args.Context.RespondAsync(embed: embed);
		}
	}
}
