using Newtonsoft.Json;
using System;

namespace Faforever.Qai.Discord.Core.Structures.Configurations
{
    /// <summary>
    /// Holds configuration infromation for a Discord Bot
    /// </summary>
    public class DiscordBotConfiguration
    {
        /// <summary>
        /// Enables slash commands for the bot.
        /// </summary>
        public bool EnableSlashCommands { get; set; }

        /// <summary>
        /// The Bot Token used to connect to Discord.
        /// </summary>
        public string Token { get; set; } = default!;

        /// <summary>
        /// The default prefix reconized by the bot.
        /// </summary>
        public string Prefix { get; set; } = default!;

        /// <summary>
        /// The default amount of shards to load. Set to 1 for automatic sharding.
        /// </summary>
        public int Shards { get; set; }

        /// <summary>
        /// The roles that are considered FAF Staff.
        /// </summary>
        public ulong[] FafStaff { get; set; } = Array.Empty<ulong>();
        
    }
}