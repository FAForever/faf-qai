using System;
using System.Collections.Generic;
using System.Linq;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Services
{
    public class PlayerStatsCalculator
    {
        public (Dictionary<string, FactionStats> factionStats, Dictionary<string, MapStats> mapStats, 
            PlayerPerformanceStats performance, PlayerActivityStats activity, List<RecentGameInfo> recentGames) 
            CalculateDetailedStats(IEnumerable<Game> games, LeaderboardRatingJournal[] globalHistory, LeaderboardRatingJournal[] ladderHistory, int playerId)
        {
            // Initialize all stats objects
            var factionStats = new Dictionary<string, FactionStats>();
            var mapStats = new Dictionary<string, MapStats>();
            var performance = new PlayerPerformanceStats();
            var activity = new PlayerActivityStats();
            var recentGames = new List<RecentGameInfo>();
            
            // Initialize performance stats from rating history
            if (globalHistory.Any())
            {
                var peakGlobal = globalHistory.OrderByDescending(r => r.AfterRating).First();
                performance.PeakGlobalRating = peakGlobal.AfterRating;
                performance.PeakGlobalDate = peakGlobal.ScoreTime;
            }

            if (ladderHistory.Any())
            {
                var peakLadder = ladderHistory.OrderByDescending(r => r.AfterRating).First();
                performance.PeakLadderRating = peakLadder.AfterRating;
                performance.PeakLadderDate = peakLadder.ScoreTime;
            }

            // Sort games once for performance streak calculations
            var gamesList = games.OrderBy(g => g.StartTime).ToList();
            
            // Initialize tracking variables
            var now = DateTime.UtcNow;
            var gamesByHour = new Dictionary<string, int>();
            var opponentCounts = new Dictionary<string, int>();
            var mapCounts = new Dictionary<string, int>();
            int currentWinStreak = 0, currentLossStreak = 0;
            int maxWinStreak = 0, maxLossStreak = 0;
            var totalDuration = TimeSpan.Zero;

            // Single loop through all games
            foreach (var game in gamesList)
            {
                var playerStats = game.PlayerStats.FirstOrDefault(ps => ps.Player.Id == playerId);
                if (playerStats == null) continue;

                // === FACTION STATS ===
                var factionName = playerStats.FactionName;
                if (!factionStats.ContainsKey(factionName))
                {
                    factionStats[factionName] = new FactionStats();
                }

                var fStats = factionStats[factionName];
                fStats.GamesPlayed++;
                fStats.TotalPlayTime = fStats.TotalPlayTime.Add(game.GameDuration);

                if (game.IsValid)
                {
                    if (playerStats.Score > 0)
                        fStats.Wins++;
                    else
                        fStats.Losses++;
                }

                if (playerStats.BeforeRating.HasValue)
                {
                    fStats.AverageRating = (fStats.AverageRating * (fStats.GamesPlayed - 1) + playerStats.BeforeRating.Value) / fStats.GamesPlayed;
                }

                // === MAP STATS ===
                var mapName = game.MapVersion?.Map?.DisplayName ?? "Unknown";
                if (!mapStats.ContainsKey(mapName))
                {
                    mapStats[mapName] = new MapStats();
                }

                var mStats = mapStats[mapName];
                mStats.GamesPlayed++;
                mStats.TotalPlayTime = mStats.TotalPlayTime.Add(game.GameDuration);
                mStats.LastPlayed = game.StartTime > mStats.LastPlayed ? game.StartTime : mStats.LastPlayed;

                if (game.IsValid)
                {
                    if (playerStats.Score > 0)
                        mStats.Wins++;
                    else
                        mStats.Losses++;
                }

                if (playerStats.BeforeRating.HasValue)
                {
                    mStats.AverageRating = (mStats.AverageRating * (mStats.GamesPlayed - 1) + playerStats.BeforeRating.Value) / mStats.GamesPlayed;
                }

                // === PERFORMANCE STATS ===
                totalDuration = totalDuration.Add(game.GameDuration);
                mapCounts[mapName] = mapCounts.GetValueOrDefault(mapName, 0) + 1;

                // Calculate streaks (only for valid games)
                if (game.IsValid)
                {
                    if (playerStats.Score > 0) // Win
                    {
                        currentWinStreak++;
                        currentLossStreak = 0;
                        maxWinStreak = Math.Max(maxWinStreak, currentWinStreak);
                    }
                    else // Loss
                    {
                        currentLossStreak++;
                        currentWinStreak = 0;
                        maxLossStreak = Math.Max(maxLossStreak, currentLossStreak);
                    }
                }

                // === ACTIVITY STATS ===
                activity.TotalGamesPlayed++;
                activity.TotalPlayTime = activity.TotalPlayTime.Add(game.GameDuration);

                if (game.IsValid)
                {
                    if (playerStats.Score > 0)
                        activity.TotalWins++;
                    else
                        activity.TotalLosses++;
                }

                // Recent activity
                var daysSince = (now - game.StartTime).TotalDays;
                if (daysSince <= 7) activity.GamesLast7Days++;
                if (daysSince <= 30) activity.GamesLast30Days++;

                // Activity by hour
                var hour = game.StartTime.Hour.ToString("D2");
                gamesByHour[hour] = gamesByHour.GetValueOrDefault(hour, 0) + 1;

                // Track opponents
                foreach (var opponent in game.PlayerStats.Where(ps => ps.Player.Id != playerId))
                {
                    opponentCounts[opponent.Player.Login] = opponentCounts.GetValueOrDefault(opponent.Player.Login, 0) + 1;
                }

                // === RECENT GAMES (first 10) ===
                if (recentGames.Count < 10)
                {
                    var gameInfo = new RecentGameInfo
                    {
                        GameId = game.Id,
                        Date = game.StartTime,
                        MapName = mapName,
                        Result = playerStats.Score > 0 ? "Victory" : (playerStats.Score < 0 ? "Defeat" : "Draw"),
                        RatingBefore = playerStats.BeforeRating ?? 0,
                        RatingAfter = playerStats.AfterRating ?? 0,
                        Faction = playerStats.FactionName,
                        Duration = game.GameDuration,
                        GameMode = game.FeaturedMod?.DisplayName ?? "Unknown"
                    };
                    recentGames.Add(gameInfo);
                }
            }

            // Finalize performance stats
            performance.LongestWinStreak = maxWinStreak;
            performance.LongestLossStreak = maxLossStreak;
            performance.AverageGameDuration = gamesList.Count > 0 ? (decimal)totalDuration.TotalMinutes / gamesList.Count : 0;
            
            if (mapCounts.Any())
            {
                var mostPlayedMap = mapCounts.OrderByDescending(kvp => kvp.Value).First();
                performance.MostPlayedMap = mostPlayedMap.Key;
                performance.MostPlayedMapCount = mostPlayedMap.Value;
            }

            // Finalize activity stats
            activity.GamesByHour = gamesByHour;
            activity.FavoriteOpponents = opponentCounts.OrderByDescending(kvp => kvp.Value)
                .Take(5).Select(kvp => $"{kvp.Key} ({kvp.Value} games)").ToList();

            return (factionStats, mapStats, performance, activity, recentGames);
        }
    }
}