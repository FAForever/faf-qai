
using System;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands
{
	public class DiscordCommandModule : ModuleBase<DiscordCommandContext>
	{
		// Use this for discord specific command tools, such as default formatting, standard complex responses, etc.

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

			await this.Context.Channel.SendMessageAsync(
				embed: SuccessBase()
					.WithColor(Context.DostyaRed)
					.WithDescription(message)
				);
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

			await this.Context.Channel.SendMessageAsync(
				embed: ErrorBase()
					.WithTitle(DiscordEmoji.FromName(this.Context.Client, ":x:"))
					.WithDescription(message)
				);
		}
		#endregion

		// Commands here would be for things such as getting a basic embed with a consistent format,
		// or any other utility that would be used across commands that is not a service.
		#region Helper Methods
		public static DiscordEmbedBuilder SuccessBase()
		{
			return new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Red);
		}

		public static DiscordEmbedBuilder ErrorBase()
		{
			return new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Black);
		}
		#endregion
	}
}
