using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Faforever.Qai.Core;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Structures.Configurations;

using IrcDotNet;

using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Irc
{
    public sealed class QaIrc : IDisposable
    {
        private readonly string _hostname;
        private readonly IrcRegistrationInfo _userInfo;
        private readonly ILogger _logger;
        private readonly QCommandsHandler _commandHandler;
        private readonly RelayService _relay;
        private readonly IServiceProvider _services;
        private readonly string[] _channels;
        private StandardIrcClient _client;
        private Thread _heartbeatThread;
        private DateTime? nextConnectAttempt = null;

        public QaIrc(IrcConfiguration config, IrcRegistrationInfo userInfo, ILogger<QaIrc> logger,
            QCommandsHandler commandHandler, RelayService relay, IServiceProvider services)
        {
            _hostname = config.Connection;
            _userInfo = userInfo;
            _logger = logger;
            _commandHandler = commandHandler;
            _relay = relay;
            _relay.DiscordMessageReceived += BounceToIRC;
            _services = services;
            _channels = config.Channels;

            InitializeClient();
        }

        public void Run()
        {
            connecting = true;
            var hostEntry = Dns.GetHostEntry(_hostname);
            var address = hostEntry.AddressList[0];
            var port = 6667;
            _logger.LogInformation("Connecting to IRC server {0} ({1}), port {2}", _hostname, address, port);
            _client.Connect(_hostname, false, _userInfo);
            
            _logger.Log(LogLevel.Debug, "Starting heartbeat thread...");
            _heartbeatThread = new Thread(HeartbeatThread);
            _heartbeatThread.Start();
        }

        public void Dispose()
        {
            DisposeClient();
        }

        private void DisposeClient()
        {
            if (_client == null)
                return;

            if (_client.IsConnected)
                _client.Quit(1000, "I'm outta here");

            _client.ErrorMessageReceived -= OnClientErrorMessageReceived;
            _client.Connected -= OnClientConnected;
            _client.ConnectFailed -= OnClientConnectFailed;
            _client.Disconnected -= OnClientDisconnected;
            _client.Registered -= OnClientRegistered;
            _client.Error -= OnError;
            _client.SaslMessage -= OnSaslMessage;
            _client.Dispose();
            _client = null;
        }

        private void InitializeClient()
        {
            _client = new StandardIrcClient { FloodPreventer = new IrcStandardFloodPreventer(4, 2000) };
            _client.ErrorMessageReceived += OnClientErrorMessageReceived;
            _client.Connected += OnClientConnected;
            _client.ConnectFailed += OnClientConnectFailed;
            _client.Disconnected += OnClientDisconnected;
            _client.Registered += OnClientRegistered;
            _client.Error += OnError;
            _client.SaslMessage += OnSaslMessage;
        }

        private async void OnPrivateMessage(object receiver, IrcMessageEventArgs eventArgs)
        {
            try
            {
                await HandleOnPrivateMessage(receiver, eventArgs);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Unhandled exception");
            }
        }

        private async Task HandleOnPrivateMessage(object receiver, IrcMessageEventArgs eventArgs)
        {
            IrcUser user = eventArgs.Source as IrcUser;

            var ctx = new IrcCommandContext(_client, eventArgs.Source.Name, user, eventArgs.Text, "!", _services);

            await _commandHandler.MessageRecivedAsync(ctx, eventArgs.Text);
        }

        private async void OnChannelMessageReceived(object sender, IrcMessageEventArgs eventArgs)
        {
            try
            {
                await ProcessOnChannelMessageReceived(sender, eventArgs);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Unhandled exception");
            }
        }

        private async Task ProcessOnChannelMessageReceived(object sender, IrcMessageEventArgs eventArgs)
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
            connecting = false;
            IrcClient client = sender as IrcClient;
            _logger.Log(LogLevel.Information, "Client registered");

            if (client == null) return;

            _logger.Log(LogLevel.Information, client.WelcomeMessage);

            _client.LocalUser.MessageReceived += OnPrivateMessage;

            client.LocalUser.JoinedChannel += (o, eventArgs) =>
            {
                _logger.Log(LogLevel.Information, "Join channel {channel}", eventArgs.Channel.Name);
                eventArgs.Channel.MessageReceived += OnChannelMessageReceived;
            };

            foreach (var channel in _channels)
            {
                client.Channels.Join($"#{channel.Trim('#')}");
            }
        }

        private bool connecting;
        private void OnClientDisconnected(object sender, EventArgs args)
        {
            _logger.Log(LogLevel.Critical, "client disconnected");
            connecting = false;
        }

        private void OnClientConnectFailed(object sender, IrcErrorEventArgs args)
        {
            _logger.Log(LogLevel.Critical, args.Error, "connect failed");
            connecting = false;
        }

        private void OnError(object sender, IrcErrorEventArgs args)
        {
            _logger.Log(LogLevel.Information, args.Error, "Error!");

            if (!_client.IsConnected)
                connecting = false;
        }

        private void OnSaslMessage(object sender, IrcSaslMessageEventArgs e)
        {
            _logger.Log(LogLevel.Information, "SASL Message: {message}", e.Message);
            if (e.Code == 904)
                nextConnectAttempt = DateTime.Now.AddSeconds(10);
        }

        private void OnClientConnected(object sender, EventArgs args)
        {
            connecting = false;
            _logger.Log(LogLevel.Information, "client connected");
        }

        private void OnClientErrorMessageReceived(object sender, IrcErrorMessageEventArgs args)
        {
            _logger.Log(LogLevel.Error, args.Message);

            if (args.Message.Contains("Throttled") || args.Message.Contains("connect too many times"))
                nextConnectAttempt = DateTime.Now.AddMinutes(1);
        }

        private Task BounceToIRC(string channel, string author, string message)
        {
            _client.LocalUser?.SendMessage(channel, $"{author}: {message}");

            return Task.CompletedTask;
        }

        private void TryReconnect()
        {
            if (nextConnectAttempt != null && DateTime.Now < nextConnectAttempt)
                return;

            nextConnectAttempt = null;
            connecting = true;
            _logger.Log(LogLevel.Information, "Trying to reconnect...");

            DisposeClient();
            InitializeClient();
            _client.Connect(_hostname, false, _userInfo);
        }

        private void HeartbeatThread()
        {
            const int PING_INTERVAL = 60;

            _logger.LogInformation("Heartbeat thread started");
            // Ping the server regularly to detect if the socket is dead

            DateTime nextPing = DateTime.Now.AddSeconds(PING_INTERVAL);

            try
            {
                while (true)
                {
                    if (!_client.IsConnected)
                    {
                        _logger.LogDebug("Client is not connected");
                        if (!connecting)
                        {
                            _logger.LogInformation("Attempting to reconnect");
                            TryReconnect();
                        }
                    }
                    else
                    {
                        if (nextPing < DateTime.Now)
                        {
                            _logger.LogDebug("Pinging {_hostname}", _hostname);
                            _client.Ping(_hostname);

                            nextPing = DateTime.Now.AddSeconds(PING_INTERVAL);
                        }
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in heartbeat thread");
            }
        }
    }
}