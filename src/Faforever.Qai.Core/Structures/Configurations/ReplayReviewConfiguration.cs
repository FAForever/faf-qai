namespace Faforever.Qai.Core.Structures.Configurations
{
    /// <summary>
    /// Where replay review requests are consumed from, and where they are posted.
    /// </summary>
    /// <remarks>
    /// The consumer stays dormant unless both <see cref="BrokerHost"/> and
    /// <see cref="ForumChannelId"/> are set, so a deployment that knows nothing
    /// about this feature keeps behaving exactly as it did.
    /// </remarks>
    public class ReplayReviewConfiguration
    {
        public string? BrokerHost { get; set; }
        public int BrokerPort { get; set; } = 5672;
        public string? BrokerUser { get; set; }
        public string? BrokerPassword { get; set; }
        public string BrokerVirtualHost { get; set; } = "/faf-core";

        /// <summary>The topic exchange the lobby server publishes to.</summary>
        public string Exchange { get; set; } = "faf-rabbitmq";

        public string RoutingKey { get; set; } = "request.replay_review.create";

        /// <summary>
        /// The queue this bot consumes. Durable and named, so requests that
        /// arrive while the bot is restarting are waiting when it comes back.
        /// </summary>
        public string QueueName { get; set; } = "faf-rabbitmq.qai.replay_review.create";

        /// <summary>The forum channel review requests are posted into.</summary>
        public ulong ForumChannelId { get; set; }

        public bool IsEnabled =>
            !string.IsNullOrWhiteSpace(BrokerHost) && ForumChannelId != 0;
    }
}
