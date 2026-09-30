using System;

namespace Faforever.Qai
{
    public class BotConfig
    {
        public string DataSource { get; set; }
        public IrcSettings Irc { get; set; } = new IrcSettings();
        public TwitchSettings Twitch { get; set; } = new TwitchSettings();
        public FafSettings Faf { get; set; } = new FafSettings();
        public FafApiSettings FafApi { get; set; } = new FafApiSettings();
        public DiscordSettings Discord { get; set; } = new DiscordSettings();
        public ReplayReviewSettings ReplayReview { get; set; } = new ReplayReviewSettings();
        
        public string BotPrefix { get; set; } = "!";
        public string Host { get; set; }

        public class IrcSettings
        {
            public string Connection { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
            public string Channels { get; set; } = "#aeolus,#newbie";
        }

        public class FafSettings
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Api { get; set; }
            public string Callback { get; set; }
        }

        public class FafApiSettings
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Endpoint { get; set; }
            public string TokenEndpoint { get; set; }
        }

        public class DiscordSettings
        {
            public bool SlashCommands { get; set; }
            public string Callback { get; set; }
            public string Api { get; set; }
            public string Scope { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string TokenEndpoint { get; set; }
            public string Token { get; set; }
            public RolesSettings Roles { get; set; }
        }

        /// <summary>
        /// Replay review requests, consumed from the lobby server's exchange.
        /// Leaving Broker or ForumChannelId unset keeps the consumer dormant,
        /// which is what every deployment does until it is switched on.
        /// </summary>
        public class ReplayReviewSettings
        {
            public string Broker { get; set; }
            public int BrokerPort { get; set; } = 5672;
            public string User { get; set; }
            public string Password { get; set; }
            public string VirtualHost { get; set; } = "/faf-core";
            public string Exchange { get; set; } = "faf-rabbitmq";
            public string RoutingKey { get; set; } = "request.replay_review.create";
            public string QueueName { get; set; } = "faf-rabbitmq.qai.replay_review.create";
            public ulong ForumChannelId { get; set; }
        }

        public class RolesSettings
        {
            public ulong[] FafStaff { get; set; } = Array.Empty<ulong>();
        }

        public class TwitchSettings
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

        
    }
}
