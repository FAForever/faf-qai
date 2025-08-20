using System;
using System.Collections.Generic;
using System.Linq;

namespace Faforever.Qai.Core.Models
{
    public class CurrentGameResult
    {
        public string GameId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public string HostUsername { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public List<GamePlayer> Players { get; set; } = new();
        public int TeamCount { get; set; }

        public List<GamePlayer> GetTeamPlayers(int team)
        {
            return Players.Where(p => p.Team == team).ToList();
        }

        public List<GamePlayer> GetObservers()
        {
            return Players.Where(p => p.Team == -1).ToList();
        }

        public List<GamePlayer> GetActualTeamPlayers()
        {
            return Players.Where(p => p.Team > 0).ToList();
        }

        public bool IsActive => StartTime > DateTime.UtcNow.AddHours(-6); // Game is considered active if started within last 6 hours and no end time
    }

    public class GamePlayer
    {
        public int FafId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Team { get; set; }
        public ulong? DiscordId { get; set; } // Resolved from AccountLink
    }

    public class ChatGroupResult
    {
        public CurrentGameResult Game { get; set; } = new();
        public List<CreatedChannel> CreatedChannels { get; set; } = new();
        public List<string> UnresolvedPlayers { get; set; } = new();
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class CreatedChannel
    {
        public int TeamNumber { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty; // "Discord" or "IRC"
        public ulong? DiscordChannelId { get; set; }
        public List<string> MovedPlayers { get; set; } = new();
    }
}