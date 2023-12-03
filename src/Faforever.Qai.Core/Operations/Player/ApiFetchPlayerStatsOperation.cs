using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using System.Threading.Tasks;
using Faforever.Qai.Core.Constants;
using Faforever.Qai.Core.Models;

using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Player
{
    [ExcludeFromCodeCoverage]
    public class ApiFetchPlayerStatsOperation : IFetchPlayerStatsOperation
    {
        private readonly FafApiClient _api;

        public ApiFetchPlayerStatsOperation(FafApiClient api)
        {
            this._api = api;
        }

        public async Task<FetchPlayerStatsResult?> FetchPlayer(string username)
        {
            var query = new ApiQuery<LeaderboardRating>()
                .Where("player.login", username)
                .Include("leaderboard,player,player.clanMembership.clan,player.names,player,player.avatarAssignments.avatar");
                

            var playerRatings = await this._api.GetAsync(query);
            if (!playerRatings.Any())
                return null;

            var player = playerRatings.FirstOrDefault()?.Player;
            if (player is null)
                return null;

            var result = new FetchPlayerStatsResult
            {
                Name = username,
                Id = player.Id.ToString(),
                LastSeen = player.UpdateTime,
            };

            SetGameStatistics(result, playerRatings);

            result.Clan = player.ClanMembership?.FirstOrDefault()?.Clan;

            result.OldNames = player.Names?.Select(n => n.Name)?.ToList() ?? new List<string>();

            return result;
        }

        public async Task<LeaderboardRatingJournal[]> FetchRatingHistory(string username, FafLeaderboard leaderboard)
        {
            var query = new ApiQuery<LeaderboardRatingJournal>()
               .Where("leaderboard.id", (int)leaderboard)
               .Where("gamePlayerStats.player.login", username)
               .Sort("-createTime");

            var ratingHistory = await this._api.GetAsync(query);
            var first = ratingHistory.FirstOrDefault();
            if (first is null)
                return Array.Empty<LeaderboardRatingJournal>();

            return ratingHistory.ToArray();
        }

        private void SetGameStatistics(FetchPlayerStatsResult result, IEnumerable<LeaderboardRating> ratings)
        {
            foreach (var rating in ratings ?? Enumerable.Empty<LeaderboardRating>())
            {
                var stats = new GameStatistics
                {
                    Rating = rating.Rating ?? 0m,
                    GamesPlayed = rating.TotalGames,
                    Ranking = 0,
                };

                if (rating.Leaderboard.TechnicalName == "ladder_1v1")
                    result.LadderStats = stats;
                else if (rating.Leaderboard.TechnicalName == "global")
                    result.GlobalStats = stats;
            }
        }
    }
}