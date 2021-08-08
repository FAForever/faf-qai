using System;
using System.Threading.Tasks;

using Faforever.Qai.Core;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Structures.Configurations;

using IrcDotNet;

using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Irc
{
    public class QaIrc : IDisposable
    {
        private readonly string _hostname;
        private readonly IrcRegistrationInfo _userInfo;
        private readonly ILogger _logger;
        private readonly QCommandsHandler _commandHandler;
        private readonly RelayService _relay;
        private readonly IServiceProvider _services;
        private readonly string[] _channels;
        private readonly IPlayerService _playerService;
        private readonly StandardIrcClient _client;

        public QaIrc(IrcConfiguration config, IrcRegistrationInfo userInfo, ILogger<QaIrc> logger,
            QCommandsHandler commandHandler, RelayService relay, IPlayerService playerService, IServiceProvider services)
        {
            _hostname = config.Connection;
            _userInfo = userInfo;
            _logger = logger;
            _commandHandler = commandHandler;
            _relay = relay;
            _relay.DiscordMessageReceived += BounceToIRC;
            _services = services;
            _channels = config.Channels;
            _playerService = playerService;

            _client = new StandardIrcClient { FloodPreventer = new IrcStandardFloodPreventer(4, 2000) };
            _client.ErrorMessageReceived += OnClientErrorMessageReceived;
            _client.Connected += OnClientConnected;
            _client.ConnectFailed += OnClientConnectFailed;
            _client.Registered += OnClientRegistered;
        }

        public void Run()
        {
            _client.Connect(_hostname, false, _userInfo);
        }

        public void Dispose()
        {
            _client.Quit(1000, "I'm outta here");
            _client.Dispose();
        }

        private async void OnPrivateMessage(object receiver, IrcMessageEventArgs eventArgs)
        {
            IrcUser user = eventArgs.Source as IrcUser;

            var ctx = new IrcCommandContext(_client, eventArgs.Source.Name, user, eventArgs.Text, "!", _services);

            await _commandHandler.MessageRecivedAsync(ctx, eventArgs.Text);
        }

        private async void OnChannelMessageReceived(object sender, IrcMessageEventArgs eventArgs)
        {
            IrcChannel channel = sender as IrcChannel;

            var logMessage = $"{channel?.Name} Received Message '{eventArgs.Text}' from '{eventArgs.Source.Name}'";
            if (channel is not null)
                logMessage += $" in channel '{channel.Name}'";

            _logger.Log(LogLevel.Debug, logMessage);

            var channeluser = channel.GetChannelUser(eventArgs.Source as IrcUser);

            if (eventArgs.Source.Name == _userInfo.NickName)
            {
                return;
            }

            var ctx = new IrcCommandContext(_client, eventArgs.Source.Name, channeluser.User, eventArgs.Text, "!", _services, channel);

            await _commandHandler.MessageRecivedAsync(ctx, eventArgs.Text);

            await _relay.IRC_MessageReceived(channel.Name, eventArgs.Source.Name, eventArgs.Text);
        }

        private void OnClientRegistered(object sender, EventArgs args)
        {
            IrcClient client = sender as IrcClient;
            _logger.Log(LogLevel.Information, "Client registered");

            if (client == null) return;

            _logger.Log(LogLevel.Information, client.WelcomeMessage);

            _client.LocalUser.MessageReceived += OnPrivateMessage;

            client.LocalUser.JoinedChannel += (o, eventArgs) =>
            {
                _logger.Log(LogLevel.Information, $"Join channel {eventArgs.Channel.Name}");
                eventArgs.Channel.MessageReceived += OnChannelMessageReceived;
            };

            foreach (var channel in _channels)
            {
                client.Channels.Join($"#{channel.Trim('#')}");
            }
        }

        private void OnClientConnectFailed(object sender, IrcErrorEventArgs args)
        {
            _logger.Log(LogLevel.Critical, args.Error, "connect failed");
        }

        private void OnClientConnected(object sender, EventArgs args)
        {
            _logger.Log(LogLevel.Information, "client connected");
        }

        private void OnClientErrorMessageReceived(object sender, IrcErrorMessageEventArgs args)
        {
            _logger.Log(LogLevel.Error, args.Message);
        }

        private Task BounceToIRC(string channel, string author, string message)
        {
            _client.LocalUser.SendMessage(channel, $"{author}: {message}");

            return Task.CompletedTask;
        }
    }
}