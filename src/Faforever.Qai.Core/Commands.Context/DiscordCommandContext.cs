using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

		public DiscordMessage Message { get; private set; }

		public DiscordCommandContext(DiscordMessage msg, string prefix, IServiceProvider services) : base(services)
		{
			Prefix = prefix;
			Message = msg;
		}

		public override async Task ReplyAsync(string message)
		{
			await Channel.SendMessageAsync(message);
		}
	}
}
