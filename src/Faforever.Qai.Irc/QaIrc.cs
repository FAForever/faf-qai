using System;
using IrcDotNet;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Irc {
	public class QaIrc : IDisposable {
		private readonly string _hostname;
		private readonly IrcRegistrationInfo _userInfo;
		private readonly ILogger _logger;

		private readonly StandardIrcClient _client;

		public QaIrc(string hostname, IrcRegistrationInfo userInfo, ILogger<QaIrc> logger) {
			_hostname = hostname;
			_userInfo = userInfo;
			_logger = logger;
			_client = new StandardIrcClient {FloodPreventer = new IrcStandardFloodPreventer(4, 2000)};
			_client.ErrorMessageReceived += (sender, args) => { _logger.Log(LogLevel.Error, args.Message); };
			_client.Connected += (sender, args) => { _logger.Log(LogLevel.Information, "client connected"); };
			_client.ConnectFailed += (sender, args) => {
				_logger.Log(LogLevel.Critical, args.Error, "connect failed");
			};

			_client.Registered += OnClientRegistered;
		}

		public bool IsRunning { get; set; }

		public void Run() {
			_client.Connect(_hostname, false, _userInfo);
			IsRunning = true;
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

		private void OnChannelMessageReceived(object? sender1, IrcMessageEventArgs messageEventArgs) {
			_logger.Log(LogLevel.Information, $"Received Message '{messageEventArgs.Text}' from '{messageEventArgs.Source.Name}");
		}
	}
}