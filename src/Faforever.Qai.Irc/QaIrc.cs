using System;
using Faforever.Qai.Core;
using Faforever.Qai.Core.Commands.Context;
using IrcDotNet;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Irc {
	public class QaIrc : IDisposable {
		private readonly string _hostname;
		private readonly IrcRegistrationInfo _userInfo;
		private readonly ILogger _logger;
		private readonly QCommandsHandler _commandHandler;
		private readonly IServiceProvider _services;

		private readonly StandardIrcClient _client;

		public QaIrc(string hostname, IrcRegistrationInfo userInfo, ILogger<QaIrc> logger,
			QCommandsHandler commandHandler, IServiceProvider services) {
			_hostname = hostname;
			_userInfo = userInfo;
			_logger = logger;
			_commandHandler = commandHandler;
			_services = services;

			_client = new StandardIrcClient {FloodPreventer = new IrcStandardFloodPreventer(4, 2000)};
			_client.ErrorMessageReceived += OnClientErrorMessageReceived;
			_client.Connected += OnClientConnected;
			_client.ConnectFailed += OnClientConnectFailed;
			_client.Registered += OnClientRegistered;
		}

		public void Run() {
			_client.Connect(_hostname, false, _userInfo);
		}

		public void Dispose() {
			_client.Quit(1000, "I'm outta here");
			_client.Dispose();
		}

		private async void OnPrivateMessage(object? receiver, IrcMessageEventArgs eventArgs) {
			var ctx = new IRCCommandContext(_client.LocalUser, eventArgs.Source.Name, eventArgs.Text, "!", _services);
			await _commandHandler.MessageRecivedAsync(ctx, eventArgs.Text);
		}

		private void OnChannelMessageReceived(object sender, IrcMessageEventArgs messageEventArgs) {
			_logger.Log(LogLevel.Information,
				$"Received Message '{messageEventArgs.Text}' from '{messageEventArgs.Source.Name}");

			IrcChannel channel = sender as IrcChannel;

			if (messageEventArgs.Source.Name == _userInfo.NickName) {
				return;
			}

			//TODO Handle this
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