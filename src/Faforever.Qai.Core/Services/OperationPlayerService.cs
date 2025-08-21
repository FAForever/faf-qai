using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Faforever.Qai.Core.Clients.QuickChart;
using Faforever.Qai.Core.Constants;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;
using Faforever.Qai.Core.Operations.Player;

namespace Faforever.Qai.Core.Services
{
    [ExcludeFromCodeCoverage]
    public class OperationPlayerService : IPlayerService
    {
        private readonly FafApiClient _api;
        private readonly GameService _gameService;
        private readonly IFetchPlayerStatsOperation _playerStatsOperation;
        private readonly IFindPlayerOperation _findPlayerOperation;
        private readonly QuickChartClient _qcClient;

        public OperationPlayerService(FafApiClient api, GameService gameService, IFetchPlayerStatsOperation playerStatsOperation, IFindPlayerOperation findPlayerOperation, QuickChartClient qcClient)
        {
            _api = api;
            _gameService = gameService;
            _playerStatsOperation = playerStatsOperation;
            _findPlayerOperation = findPlayerOperation;
            _qcClient = qcClient;
        }

        public Task<FetchPlayerStatsResult?> FetchPlayerStats(string username)
        {
            return _playerStatsOperation.FetchPlayer(username);
        }

        public Task<FindPlayerResult> FindPlayer(string searchTerm)
        {
            return _findPlayerOperation.FindPlayer(searchTerm);
        }

        public Task<LeaderboardRatingJournal[]> GetRatingHistory(string username, FafLeaderboard leaderboard)
        {
            return _playerStatsOperation.FetchRatingHistory(username, leaderboard);
        }

        public async Task<LastSeenPlayerResult?> LastSeenPlayer(string username)
        {
            var lastGame = await _gameService.FetchLastGame(username);

            // If no last game was found we need to query the player directly
            Player? player = null;

            if (lastGame is null)
            {
                player = await FetchPlayer(username);
            }
            else
            {
                player = lastGame.PlayerStats.FirstOrDefault(ps => ps.Player.Login.Equals(username, StringComparison.InvariantCultureIgnoreCase))?.Player;
            }

            if (player == null)
                return null;

            return new LastSeenPlayerResult
            {
                Username = player.Login,
                SeenFaf = player?.UpdateTime,
                SeenGame = lastGame?.EndTime ?? lastGame?.StartTime
            };
        }

        public async Task<Player?> FetchPlayer(string username)
        {
            var query = new ApiQuery<Player>()
                .Where("login", username)
                .Limit(1);

            var players = await _api.GetAsync(query);

            return players.FirstOrDefault();
        }

        public async Task<byte[]> GenerateRatingChart(string username, FafLeaderboard leaderboard)
        {
            var ratingHistory = await GetRatingHistory(username, leaderboard);
            var title = $"{username} - {leaderboard} rating";

            // Check if player has any rating history
            if (ratingHistory.Length == 0)
            {
                throw new InvalidOperationException($"Player '{username}' has no rating history for {leaderboard} leaderboard.");
            }

            // Group by week
            var groupedData = ratingHistory
                .OrderBy(r => r.ScoreTime)
                .GroupBy(r => new { r.ScoreTime.Year, Week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(r.ScoreTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday) })
                .Select(g => new {
                    WeekStart = g.Min(r => r.ScoreTime),
                    RatingData = g,
                    GamesPlayed = g.Count()
                });
            var maxGamesPlayed = groupedData.Max(g => g.GamesPlayed);

            // Process grouped data
            var globalRatingData = new List<CandleStickDataPoint>();
            var globalGamesPlayed = new List<DataPoint>();
            var averageRating = new List<DataPoint>();

            foreach (var group in groupedData)
            {
                var open = group.RatingData.First().BeforeRating;
                var close = group.RatingData.Last().AfterRating;
                var high = group.RatingData.Max(r => r.AfterRating);
                var low = group.RatingData.Min(r => r.AfterRating);
                var timestamp = new DateTimeOffset(group.WeekStart).ToUnixTimeMilliseconds();
                var average = group.RatingData.Average(r => r.AfterRating);

                globalRatingData.Add(new CandleStickDataPoint { Open = open, High = high, Low = low, Close = close, Timestamp = timestamp });
                globalGamesPlayed.Add(new DataPoint { Value = group.GamesPlayed, Timestamp = timestamp });
                averageRating.Add(new DataPoint { Value = average, Timestamp = timestamp });
            }

            var req = ChartTemplate.CreateRatingChartRequest(title);
            req.Chart.Data.Datasets[0].Data = globalRatingData.ToArray();
            req.Chart.Data.Datasets[1].Data = globalGamesPlayed.ToArray();
            req.Chart.Data.Datasets[2].Data = averageRating.ToArray();
            if (req.Chart.Options?.Scales?.Volume is not null)
                req.Chart.Options.Scales.Volume.Max = maxGamesPlayed * 7;

            var chartBytes = await _qcClient.GetChartAsync(req);

            return chartBytes;
        }

        public async Task<DetailedPlayerStatsResult?> FetchDetailedPlayerStats(string username)
        {
            // Get basic player info
            var basicStats = await FetchPlayerStats(username);
            if (basicStats == null) return null;

            // Get player data with extended info
            var playerQuery = new ApiQuery<Player>()
                .Where("login", username)
                .Include("names")
                .Limit(1);
            var playerData = (await _api.GetAsync(playerQuery)).FirstOrDefault();
            if (playerData == null) return null;

            // Get recent games for detailed analysis (last 1000+ games)
            var gamesQuery = new ApiQuery<Game>()
                .Where("playerStats.player.login", username)
                .Include("playerStats.player,mapVersion.map,featuredMod")
                .Sort("-startTime")
                .Limit(1001);
            var gamesList = (await _api.GetAsync(gamesQuery)).ToList();
            
            // If we got exactly 1001 games, remove the last one and indicate 1000+
            var hasMoreGames = gamesList.Count == 1001;
            if (hasMoreGames)
            {
                gamesList.RemoveAt(gamesList.Count - 1);
            }
            var games = gamesList.AsEnumerable();

            // Get rating history for both leaderboards
            var globalHistory = await GetRatingHistory(username, FafLeaderboard.Global);
            var ladderHistory = await GetRatingHistory(username, FafLeaderboard.Ladder1v1);

            var result = new DetailedPlayerStatsResult
            {
                Name = basicStats.Name,
                Id = basicStats.Id,
                LastSeen = basicStats.LastSeen,
                AccountCreated = playerData.CreatedTime,
                LadderStats = basicStats.LadderStats,
                GlobalStats = basicStats.GlobalStats,
                Clan = basicStats.Clan,
                OldNames = basicStats.OldNames,
                HasMoreThan1000Games = hasMoreGames,
            };

            // Calculate detailed statistics
            result.FactionStatistics = CalculateFactionStats(games, username);
            result.MapStatistics = CalculateMapStats(games, username);
            result.Performance = CalculatePerformanceStats(games, globalHistory, ladderHistory, username);
            result.Activity = CalculateActivityStats(games, username);
            result.RecentGames = GetRecentGameInfo(games.Take(10), username);

            return result;
        }

        private Dictionary<string, FactionStats> CalculateFactionStats(IEnumerable<Game> games, string username)
        {
            var factionStats = new Dictionary<string, FactionStats>();

            foreach (var game in games)
            {
                var playerStats = game.PlayerStats.FirstOrDefault(ps => 
                    ps.Player.Login.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                if (playerStats == null) continue;

                var factionName = playerStats.FactionName;
                if (!factionStats.ContainsKey(factionName))
                {
                    factionStats[factionName] = new FactionStats();
                }

                var stats = factionStats[factionName];
                stats.GamesPlayed++;
                stats.TotalPlayTime = stats.TotalPlayTime.Add(game.GameDuration);

                // Determine win/loss (assuming positive score = win)
                if (playerStats.Score > 0)
                    stats.Wins++;
                else
                    stats.Losses++;

                // Add to average rating calculation
                if (playerStats.BeforeRating.HasValue)
                {
                    stats.AverageRating = (stats.AverageRating * (stats.GamesPlayed - 1) + playerStats.BeforeRating.Value) / stats.GamesPlayed;
                }
            }

            return factionStats;
        }

        private Dictionary<string, MapStats> CalculateMapStats(IEnumerable<Game> games, string username)
        {
            var mapStats = new Dictionary<string, MapStats>();

            foreach (var game in games)
            {
                var playerStats = game.PlayerStats.FirstOrDefault(ps => 
                    ps.Player.Login.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                if (playerStats == null) continue;

                var mapName = game.MapVersion?.Map?.DisplayName ?? "Unknown";
                if (!mapStats.ContainsKey(mapName))
                {
                    mapStats[mapName] = new MapStats();
                }

                var stats = mapStats[mapName];
                stats.GamesPlayed++;
                stats.TotalPlayTime = stats.TotalPlayTime.Add(game.GameDuration);
                stats.LastPlayed = game.StartTime > stats.LastPlayed ? game.StartTime : stats.LastPlayed;

                // Determine win/loss
                if (playerStats.Score > 0)
                    stats.Wins++;
                else
                    stats.Losses++;

                // Add to average rating calculation
                if (playerStats.BeforeRating.HasValue)
                {
                    stats.AverageRating = (stats.AverageRating * (stats.GamesPlayed - 1) + playerStats.BeforeRating.Value) / stats.GamesPlayed;
                }
            }

            return mapStats;
        }

        private PlayerPerformanceStats CalculatePerformanceStats(IEnumerable<Game> games, 
            LeaderboardRatingJournal[] globalHistory, LeaderboardRatingJournal[] ladderHistory, string username)
        {
            var performance = new PlayerPerformanceStats();

            // Find peak ratings
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

            // Calculate streaks and other performance metrics
            var gamesList = games.OrderBy(g => g.StartTime).ToList();
            int currentWinStreak = 0, currentLossStreak = 0;
            int maxWinStreak = 0, maxLossStreak = 0;
            var totalDuration = TimeSpan.Zero;
            var mapCounts = new Dictionary<string, int>();

            foreach (var game in gamesList)
            {
                var playerStats = game.PlayerStats.FirstOrDefault(ps => 
                    ps.Player.Login.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                if (playerStats == null) continue;

                totalDuration = totalDuration.Add(game.GameDuration);

                // Track map statistics
                var mapName = game.MapVersion?.Map?.DisplayName ?? "Unknown";
                mapCounts[mapName] = mapCounts.GetValueOrDefault(mapName, 0) + 1;

                // Calculate streaks
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

            performance.LongestWinStreak = maxWinStreak;
            performance.LongestLossStreak = maxLossStreak;
            performance.AverageGameDuration = gamesList.Count > 0 ? (decimal)totalDuration.TotalMinutes / gamesList.Count : 0;

            // Most played map
            if (mapCounts.Any())
            {
                var mostPlayed = mapCounts.OrderByDescending(kvp => kvp.Value).First();
                performance.MostPlayedMap = mostPlayed.Key;
                performance.MostPlayedMapCount = mostPlayed.Value;
            }

            return performance;
        }

        private PlayerActivityStats CalculateActivityStats(IEnumerable<Game> games, string username)
        {
            var activity = new PlayerActivityStats();
            var now = DateTime.UtcNow;
            var gamesByHour = new Dictionary<string, int>();
            var opponentCounts = new Dictionary<string, int>();

            foreach (var game in games)
            {
                var playerStats = game.PlayerStats.FirstOrDefault(ps => 
                    ps.Player.Login.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                if (playerStats == null) continue;

                activity.TotalGamesPlayed++;
                activity.TotalPlayTime = activity.TotalPlayTime.Add(game.GameDuration);

                if (playerStats.Score > 0)
                    activity.TotalWins++;
                else
                    activity.TotalLosses++;

                // Recent activity
                var daysSince = (now - game.StartTime).TotalDays;
                if (daysSince <= 7) activity.GamesLast7Days++;
                if (daysSince <= 30) activity.GamesLast30Days++;

                // Activity by hour
                var hour = game.StartTime.Hour.ToString("D2");
                gamesByHour[hour] = gamesByHour.GetValueOrDefault(hour, 0) + 1;

                // Track opponents
                foreach (var opponent in game.PlayerStats.Where(ps => ps.Player.Login != username))
                {
                    opponentCounts[opponent.Player.Login] = opponentCounts.GetValueOrDefault(opponent.Player.Login, 0) + 1;
                }
            }

            activity.GamesByHour = gamesByHour;
            activity.FavoriteOpponents = opponentCounts.OrderByDescending(kvp => kvp.Value)
                .Take(5).Select(kvp => $"{kvp.Key} ({kvp.Value} games)").ToList();

            return activity;
        }

        private List<RecentGameInfo> GetRecentGameInfo(IEnumerable<Game> games, string username)
        {
            var recentGames = new List<RecentGameInfo>();

            foreach (var game in games)
            {
                var playerStats = game.PlayerStats.FirstOrDefault(ps => 
                    ps.Player.Login.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                if (playerStats == null) continue;

                var gameInfo = new RecentGameInfo
                {
                    GameId = game.Id,
                    Date = game.StartTime,
                    MapName = game.MapVersion?.Map?.DisplayName ?? "Unknown",
                    Result = playerStats.Score > 0 ? "Victory" : (playerStats.Score < 0 ? "Defeat" : "Draw"),
                    RatingBefore = playerStats.BeforeRating ?? 0,
                    RatingAfter = playerStats.AfterRating ?? 0,
                    Faction = playerStats.FactionName,
                    Duration = game.GameDuration,
                    GameMode = game.FeaturedMod?.DisplayName ?? "Unknown"
                };

                recentGames.Add(gameInfo);
            }

            return recentGames;
        }
    }
}