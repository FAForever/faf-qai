using System;
using System.Threading.Tasks;

using IrcDotNet;

namespace Faforever.Qai.Core.Commands.Context
{
	public class IRCCommandContext : CustomCommandContext
	{
		public string RespondTo { get; private set; }
		public IrcUser Author { get; set; }
		public IrcLocalUser Client { get; private set; }
		public IrcChannel? Channel { get; private set; }
		public IrcChannelUser? ChannelUser { get; private set; }
		public string Message { get; private set; }

		public IRCCommandContext(IrcLocalUser client, string respondTo, IrcUser author, string message, string prefix, IServiceProvider services, IrcChannel? channel = null) : base(services)
		{
			Client = client;
			RespondTo = respondTo;
			Message = message;
			Prefix = prefix;
			Author = author;

			Channel = channel;

			if (!(Channel is null))
				ChannelUser = Channel.GetChannelUser(Author);
			else ChannelUser = null;
		}

		public override Task ReplyAsync(string message)
		{
			return Task.Run(() =>
			{
				//IRC doesn't support newlines. So we replace those with spaces.
				message = message.Replace("\n", " ");
				Client.SendMessage(RespondTo, message);
			});
		}

		public override Task ActionAsync(string message)
		{
			return Task.Run(() =>
			{
				message = message.Replace("\n", " ");
				IIrcMessageTarget? target = Channel;
				if (target is null)
					target = Author;

				var actionMessage = IrcUtils.ActionMessage(message);
				Client.SendMessage(target, actionMessage);
			});
		}
	}
}
