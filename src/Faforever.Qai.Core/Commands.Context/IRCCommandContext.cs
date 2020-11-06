using System;
using System.Threading.Tasks;

using IrcDotNet;

namespace Faforever.Qai.Core.Commands.Context
{
	public class IRCCommandContext : CustomCommandContext
	{
		public string Name { get; private set; }
		public IrcLocalUser Client { get; private set; }
		public string Message { get; private set; }

		public IRCCommandContext(IrcLocalUser client, string name, string message, string prefix, IServiceProvider services) : base(services)
		{
			Client = client;
			Name = name;
			Message = message;
			Prefix = prefix;
		}

		public override Task ReplyAsync(string message)
		{
			return Task.Run(() => { Client.SendMessage(Name, message); });
		}
	}
}
