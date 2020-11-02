using System;
using System.Threading.Tasks;
using IrcDotNet;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Irc {
	public class QaIrc : IDisposable {
		private readonly string _hostname;
		private readonly IrcRegistrationInfo _userInfo;
		private readonly ILogger _logger;

		private readonly StandardIrcClient _client;
		private IrcLocalUser _user;

		public QaIrc(string hostname, IrcRegistrationInfo userInfo, ILogger<QaIrc> logger) {
			_hostname = hostname;
			_userInfo = userInfo;
			_logger = logger;
			_client = new StandardIrcClient {FloodPreventer = new IrcStandardFloodPreventer(4, 2000)};
			_client.ErrorMessageReceived += OnClientErrorMessageReceived;
			_client.Connected += OnClientConnected;
			_client.ConnectFailed += OnClientConnectFailed;
			_client.Registered += OnClientRegistered;
		}

		public void Run() {
			_client.Connect(_hostname, false, _userInfo);
			_user = _client.LocalUser;
		}

		public void Dispose() {
			_client.Quit(1000, "I'm outta here");
			_client.Dispose();
		}

		private void OnPrivateMessage(object? o, IrcMessageEventArgs eventArgs) {
			_logger.Log(LogLevel.Information, $"Got private message '{eventArgs.Text}'");
		}

		private void OnClientRegistered(object? sender, EventArgs args) {
			IrcClient client = sender as IrcClient;
			_logger.Log(LogLevel.Information, "Client registered");

			if (client == null) return;

			_logger.Log(LogLevel.Information, client.WelcomeMessage);

			_client.LocalUser.MessageReceived += OnPrivateMessage;

			client.LocalUser.JoinedChannel += (o, eventArgs) => {
				_logger.Log(LogLevel.Information, $"Join channel {eventArgs.Channel.Name}");
				eventArgs.Channel.MessageReceived += OnChannelMessageReceived;
			};

			client.Channels.Join("#aeolus");
		}

		private void OnChannelMessageReceived(object sender, IrcMessageEventArgs messageEventArgs) {
			_logger.Log(LogLevel.Information,
				$"Received Message '{messageEventArgs.Text}' from '{messageEventArgs.Source.Name}");
			
			IrcChannel channel = sender as IrcChannel;
			
			if (messageEventArgs.Source.Name == _userInfo.NickName) {
				return;
			}
			
			//TODO This message should be handled.
		}
		
		private void OnClientConnectFailed(object sender, IrcErrorEventArgs args) {
			_logger.Log(LogLevel.Critical, args.Error, "connect failed");
		}
		
		private void OnClientConnected(object sender, EventArgs args) {
			_logger.Log(LogLevel.Information, "client connected");
		}
		
		private void OnClientErrorMessageReceived(object sender, IrcErrorMessageEventArgs args) {
			_logger.Log(LogLevel.Error, args.Message);
		}
	}
}