using Faforever.Qai.Core.Operations.FafApi;
using System;
using System.Collections.Generic;

namespace Faforever.Qai.Core.Models
{
    public class FetchPlayerStatsResult
    {
        public string Name { get; set; } = default!;
        public string Id { get; set; } = default!;
        public DateTime LastSeen { get; set; } = default!;
        public GameStatistics? LadderStats { get; set; } = null;
        public GameStatistics? GlobalStats { get; set; } = null;
        public Clan? Clan { get; set; } = null;
        public List<string> OldNames { get; set; } = new List<string>();
        public ReplayData? ReplayData { get; set; } = null;
    }

    public class FetchRatingHistoryResult
    {
        public required string PlayerId { get; set; }
        public List<LeaderboardRatingJournal> RatingHistory { get; set; } = new List<LeaderboardRatingJournal>();
    }


    public struct GameStatistics
    {
        public short Ranking { get; set; }
        public decimal Rating { get; set; }
        public int GamesPlayed { get; set; }
    }

    public class FAFClan
    {
        public string? Name { get; set; }
        public int? Size { get; set; }
        public string? URL { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Description { get; set; }
        public string? Tag { get; set; }
        public long? Id { get; set; }
    }

    public struct ReplayData
    {
        public int? Team { get; set; }
        public int? Score { get; set; }
        public string? Faction { get; set; }
    }

    public class DetailedPlayerStatsResult
    {
        public string Name { get; set; } = default!;
        public string Id { get; set; } = default!;
        public DateTime LastSeen { get; set; } = default!;
        public DateTime AccountCreated { get; set; } = default!;
        public GameStatistics? LadderStats { get; set; } = null;
        public GameStatistics? GlobalStats { get; set; } = null;
        public Clan? Clan { get; set; } = null;
        public List<string> OldNames { get; set; } = new List<string>();
        
        // Detailed statistics
        public Dictionary<string, FactionStats> FactionStatistics { get; set; } = new();
        public PlayerPerformanceStats Performance { get; set; } = new();
        public PlayerActivityStats Activity { get; set; } = new();
        public List<RecentGameInfo> RecentGames { get; set; } = new();
        public Dictionary<string, MapStats> MapStatistics { get; set; } = new();
        public bool HasMoreThan1000Games { get; set; } = false;
        public string GameCountDisplay => HasMoreThan1000Games ? "1000+" : Activity.TotalGamesPlayed.ToString();
    }

    public class FactionStats
    {
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public decimal WinRate => GamesPlayed > 0 ? (decimal)Wins / GamesPlayed * 100 : 0;
        public TimeSpan TotalPlayTime { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class MapStats
    {
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public decimal WinRate => GamesPlayed > 0 ? (decimal)Wins / GamesPlayed * 100 : 0;
        public TimeSpan TotalPlayTime { get; set; }
        public DateTime LastPlayed { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class PlayerPerformanceStats
    {
        public decimal PeakGlobalRating { get; set; }
        public decimal PeakLadderRating { get; set; }
        public DateTime PeakGlobalDate { get; set; }
        public DateTime PeakLadderDate { get; set; }
        public int LongestWinStreak { get; set; }
        public int LongestLossStreak { get; set; }
        public decimal AverageGameDuration { get; set; }
        public string MostPlayedMap { get; set; } = "";
        public int MostPlayedMapCount { get; set; }
    }

    public class PlayerActivityStats
    {
        public int TotalGamesPlayed { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public decimal OverallWinRate => TotalGamesPlayed > 0 ? (decimal)TotalWins / TotalGamesPlayed * 100 : 0;
        public TimeSpan TotalPlayTime { get; set; }
        public int GamesLast7Days { get; set; }
        public int GamesLast30Days { get; set; }
        public Dictionary<string, int> GamesByHour { get; set; } = new(); // Hour of day -> game count
        public List<string> FavoriteOpponents { get; set; } = new(); // Most played against
    }

    public class RecentGameInfo
    {
        public int GameId { get; set; }
        public DateTime Date { get; set; }
        public string MapName { get; set; } = "";
        public string Result { get; set; } = ""; // "Victory", "Defeat", "Draw"
        public int RatingBefore { get; set; }
        public int RatingAfter { get; set; }
        public int RatingChange => RatingAfter - RatingBefore;
        public string Faction { get; set; } = "";
        public TimeSpan Duration { get; set; }
        public string GameMode { get; set; } = "";
    }
}