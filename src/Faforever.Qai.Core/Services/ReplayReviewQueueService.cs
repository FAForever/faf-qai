using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

using Faforever.Qai.Core.Structures.Configurations;
using Faforever.Qai.Core.Structures.ReplayReview;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Faforever.Qai.Core.Services
{
    /// <summary>
    /// Consumes replay review requests from RabbitMQ and posts them to Discord.
    /// </summary>
    /// <remarks>
    /// The bot only consumes here; it exposes no new port and accepts no
    /// inbound connection for this. A request reaches it because the lobby
    /// server put one on the exchange, and the lobby only does that for a
    /// player who is authenticated on its socket. So the player name in a post
    /// is one FAF vouched for, without this bot having to verify anything.
    ///
    /// Turning the feature off is a broker operation: unbind the queue and no
    /// request arrives, with no code change and no bot restart.
    /// </remarks>
    public sealed class ReplayReviewQueueService(
        ReplayReviewConfiguration configuration,
        DiscordRestClient rest,
        ILogger<ReplayReviewQueueService> logger) : IAsyncDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private string? _consumerTag;

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (!configuration.IsEnabled)
            {
                logger.LogInformation(
                    "Replay review requests are not configured; not consuming.");
                return;
            }

            var factory = new ConnectionFactory
            {
                HostName = configuration.BrokerHost!,
                Port = configuration.BrokerPort,
                UserName = configuration.BrokerUser ?? ConnectionFactory.DefaultUser,
                Password = configuration.BrokerPassword ?? ConnectionFactory.DefaultPass,
                VirtualHost = configuration.BrokerVirtualHost,
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: configuration.Exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: configuration.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await _channel.QueueBindAsync(
                queue: configuration.QueueName,
                exchange: configuration.Exchange,
                routingKey: configuration.RoutingKey,
                cancellationToken: cancellationToken);

            // One unacknowledged message at a time. Posting is slow compared
            // to consuming, and a review request has no reason to be handled
            // in parallel with another one.
            await _channel.BasicQosAsync(0, 1, false, cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnMessageAsync;

            _consumerTag = await _channel.BasicConsumeAsync(
                queue: configuration.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);

            logger.LogInformation(
                "Consuming replay review requests from {Queue} bound to {Exchange}/{RoutingKey}",
                configuration.QueueName,
                configuration.Exchange,
                configuration.RoutingKey);
        }

        private async Task OnMessageAsync(object sender, BasicDeliverEventArgs args)
        {
            if (_channel is null)
                return;

            ReplayReviewRequest? request;
            try
            {
                var body = Encoding.UTF8.GetString(args.Body.Span);
                request = JsonConvert.DeserializeObject<ReplayReviewRequest>(body);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Dropping replay review request with an unreadable body.");
                await RejectAsync(args, requeue: false);
                return;
            }

            if (request is null || !request.IsValid)
            {
                logger.LogWarning("Dropping replay review request that is missing required fields.");
                await RejectAsync(args, requeue: false);
                return;
            }

            try
            {
                await PostAsync(request);
            }
            catch (Exception ex) when (IsMisconfiguration(ex))
            {
                // The channel is wrong, gone, or closed to the bot. Retrying
                // cannot fix any of those, and retrying immediately would spin
                // against Discord's API for as long as the mistake stands. So
                // the request goes back on the queue and this consumer stops:
                // nothing is lost, someone fixes the configuration, and a
                // restart drains the backlog.
                logger.LogError(
                    ex,
                    "Cannot post to channel {ChannelId}; returning the request to the queue and stopping the consumer.",
                    configuration.ForumChannelId);
                await RejectAsync(args, requeue: true);
                await StopConsumingAsync();
                return;
            }
            catch (Exception ex)
            {
                // Requeue: what is left is Discord being briefly unavailable,
                // which is fixed without the player having to ask again.
                logger.LogError(
                    ex,
                    "Failed to post replay review request from player {PlayerId}; requeueing.",
                    request.PlayerId);
                await RejectAsync(args, requeue: true);
                return;
            }

            await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
        }

        /// <summary>
        /// Whether this failure is a configuration mistake rather than a blip.
        /// </summary>
        private static bool IsMisconfiguration(Exception ex) =>
            ex is InvalidOperationException
                or NotFoundException
                or UnauthorizedException;

        private async Task PostAsync(ReplayReviewRequest request)
        {
            // `CreateForumPostAsync` lives on the forum subtype, so the cast is
            // also the check that the configured channel is a forum at all.
            if (await rest.GetChannelAsync(configuration.ForumChannelId)
                is not DiscordForumChannel channel)
            {
                throw new InvalidOperationException(
                    $"Channel {configuration.ForumChannelId} is not a forum channel.");
            }

            var post = ReplayReviewPostFormatter.Format(request);

            var message = new DiscordMessageBuilder()
                .WithContent(post.Body)
                // Everything but the labels is text a player typed. It is
                // shown, never allowed to ping anyone.
                .WithAllowedMentions(Mentions.None);

            await channel.CreateForumPostAsync(
                new ForumPostBuilder()
                    .WithName(post.Title)
                    .WithMessage(message));

            logger.LogInformation(
                "Posted replay review request from player {PlayerId} for replay {ReplayId}",
                request.PlayerId,
                request.ReplayId);
        }

        private async Task StopConsumingAsync()
        {
            if (_channel is null || _consumerTag is null)
                return;

            await _channel.BasicCancelAsync(_consumerTag);
            _consumerTag = null;
        }

        private async Task RejectAsync(BasicDeliverEventArgs args, bool requeue)
        {
            if (_channel is null)
                return;

            await _channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: requeue);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
            {
                _consumerTag = null;
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection is not null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
        }
    }
}
