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
            req.Chart.Options.Scales.Volume.Max = maxGamesPlayed * 7;

            var chartBytes = await _qcClient.GetChartAsync(req);

            return chartBytes;
        }
    }
}