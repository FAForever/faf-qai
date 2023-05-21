namespace Faforever.Qai
{
    public class BotConfig
    {
        public string DataSource { get; set; }
        public IrcSettings Irc { get; set; }
        public TwitchSettings Twitch { get; set; }
        public FafSettings Faf { get; set; }
        public DiscordSettings Discord { get; set; }
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

        public class DiscordSettings
        {
            public string Callback { get; set; }
            public string Api { get; set; }
            public string Scope { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string TokenEndpoint { get; set; }
            public string Token { get; set; }
        }

        public class TwitchSettings
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}
