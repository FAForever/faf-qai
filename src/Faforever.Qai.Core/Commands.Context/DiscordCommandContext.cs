using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Faforever.Qai.Core.Commands.Context
{
	public class DiscordCommandContext : CustomCommandContext
	{
		public DiscordChannel Channel
		{
			get
			{
				return Message.Channel;
			}
		}

		public DiscordClient Client { get; private set; }
		public DiscordUser User { get; private set; }
		public DiscordMessage Message { get; private set; }
		public DiscordGuild Guild { get; private set; }

		public DiscordCommandContext(DiscordClient client, MessageCreateEventArgs args, string prefix, IServiceProvider services) : base(services)
		{
			Client = client;
			Prefix = prefix;
			Message = args.Message;
			Guild = args.Guild;
			User = args.Author;
		}

		public override async Task ReplyAsync(string message)
		{
			await Channel.SendMessageAsync(message);
		}
	}
}
