using System;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Context
{
	public enum ReplyOption
	{
		Always,
		OnlyWhenPrivate,
		InPrivate
	}

	public abstract class CustomCommandContext : CommandContext
	{
		/// <summary>
		/// The prefix used to trigger this command.
		/// </summary>
		public string Prefix { get; protected set; } = default!;
		public readonly DiscordColor DostyaRed = new DiscordColor(0xff0000);
		public CustomCommandContext(IServiceProvider services) : base(services) { }

		protected abstract bool isPrivate { get; }

		public Task ReplyAsync(object message, ReplyOption replyOption = ReplyOption.Always)
		{
			var privateResponse = isPrivate || replyOption == ReplyOption.InPrivate;

			if (privateResponse || replyOption != ReplyOption.OnlyWhenPrivate)
				return SendReplyAsync(message, privateResponse);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Reply to be sent back to the user.
		/// </summary>
		/// <param name="message">Message to be sent back.</param>
		/// <returns>Operation for this reply.</returns>
		protected abstract Task SendReplyAsync(object message, bool inPrivate = false);
		/// <summary>
		/// An action (/me) sent back to the user
		/// </summary>
		/// <param name="action">Message to be sent back.</param>
		/// <returns>Operation for this reply.</returns>
		public abstract Task SendActionAsync(string action);


	}
}
