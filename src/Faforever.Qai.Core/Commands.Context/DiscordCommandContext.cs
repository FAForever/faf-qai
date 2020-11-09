using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;

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

		public DiscordMessage Message { get; private set; }

		public DiscordCommandContext(DiscordClient client, DiscordMessage msg, string prefix, IServiceProvider services) : base(services)
		{
			Client = client;
			Prefix = prefix;
			Message = msg;
		}

		public override async Task ReplyAsync(string message)
		{
			await Channel.SendMessageAsync(message);
		}
	}
}
