using System.Threading.Tasks;
using Faforever.Qai.Core;
using IrcDotNet;

namespace Faforever.Qai.Irc {
	public class IrcCommandSource : ICommandSource {
		private readonly IrcLocalUser _client;

		public IrcCommandSource(IrcLocalUser client, string name) {
			_client = client;
			Name = name;
		}

		public string Name { get; }

		public Task Respond(string message) {
			return Task.Run(() => { _client.SendMessage(Name, message); });
		}
	}
}