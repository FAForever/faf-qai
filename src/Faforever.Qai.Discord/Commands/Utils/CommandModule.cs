using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands.Utils
{
	public class CommandModule : BaseCommandModule
	{
		private CommandContext? Context;

		public override Task BeforeExecutionAsync(CommandContext ctx)
		{
			this.Context = ctx;

			return Task.CompletedTask;
		}
		// Use these two methods to create more complex success and error responses
		// that wont have to be repeatedly built in each command class.
		#region Responders
		/// <summary>
		/// Responds with a basic success message.
		/// </summary>
		/// <param name="message">Text to send.</param>
		/// <returns></returns>
		protected async Task RespondBasicSuccess(string message)
		{
			if (this.Context is null)
				throw new NullReferenceException("Command Context is null");

			await this.Context.RespondAsync(message);
		}

		/// <summary>
		/// Responds with a basic error message.
		/// </summary>
		/// <param name="message">Text to send.</param>
		/// <returns></returns>
		protected async Task RespondBasicError(string message)
		{
			if (this.Context is null)
				throw new NullReferenceException("Command Conntext is null");

			await this.Context.RespondAsync(DiscordEmoji.FromName(this.Context.Client, ":x:") + $" {message}");
		}
		#endregion

		// Commands here would be for things such as getting a basic embed with a consistent format,
		// or any other utility that would be used across commands that is not a service.
		#region Helper Methods
		public static DiscordEmbedBuilder SuccessBase()
		{
			return new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Blue);
		}

		public static DiscordEmbedBuilder ErrorBase()
		{
			return new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Red);
		}
		#endregion
	}
}
