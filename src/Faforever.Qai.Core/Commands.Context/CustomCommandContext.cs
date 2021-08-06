using System;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Context
{
	public abstract class CustomCommandContext : CommandContext
	{
		/// <summary>
		/// The prefix used to trigger this command.
		/// </summary>
		public string Prefix { get; protected set; } = default!;

		public readonly DiscordColor DostyaRed = new DiscordColor(0xff0000);

		public CustomCommandContext(IServiceProvider services) : base(services) { }

		/// <summary>
		/// A basic text only reply to be sent back to the user.
		/// </summary>
		/// <param name="message">Message to be sent back.</param>
		/// <returns>Operation for this reply.</returns>
		public abstract Task ReplyAsync(string message);
		/// <summary>
		/// An action (/me) sent back to the user
		/// </summary>
		/// <param name="action">Message to be sent back.</param>
		/// <returns>Operation for this reply.</returns>
		public abstract Task ActionAsync(string action);
	}
}
