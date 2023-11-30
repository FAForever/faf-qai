using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Clients;
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
                Id = player.Id.ToString()
            };

            SetGameStatistics(result, playerRatings);

            result.Clan = player.ClanMembership?.FirstOrDefault()?.Clan;

            result.OldNames = player.Names?.Select(n => n.Name)?.ToList() ?? new List<string>();

            return result;
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

        /*
        public async Task<FetchPlayerStatsResult?> FetchPlayer(string username)
        {
            using Stream? stream =
                await this._api.Client.GetStreamAsync(
                    $"/data/player?include=clanMembership.clan,globalRating,ladder1v1Rating,names,avatarAssignments.avatar&filter=login=={username}");

            using JsonDocument json = await JsonDocument.ParseAsync(stream);

            if (!json.RootElement.TryGetProperty("data", out var data))
                return null;
            
            if(data.GetArrayLength() == 0)
                return null;

            FetchPlayerStatsResult result = new FetchPlayerStatsResult
            {
                Name = username,
                Id = json.RootElement.GetProperty("data")[0].GetProperty("id").GetString() ?? ""
            };

            if(json.RootElement.TryGetProperty("included", out var includedElement))
            {
                foreach (JsonElement element in includedElement.EnumerateArray())
                {
                    var typeElement = element.GetProperty("type");
                    var attributes = element.GetProperty("attributes");
                    switch (typeElement.GetString())
                    {
                        case "ladder1v1Rating":
                            var ladder = new GameStatistics
                            {
                                Ranking = attributes.GetProperty("ranking").GetInt16(),
                                Rating = attributes.GetProperty("rating").GetDecimal(),
                                GamesPlayed = attributes.GetProperty("numberOfGames").GetInt16()
                            };
                            result.LadderStats = ladder;
                            break;
                        case "globalRating":
                            var global = new GameStatistics
                            {
                                Ranking = attributes.GetProperty("ranking").GetInt16(),
                                Rating = attributes.GetProperty("rating").GetDecimal(),
                                GamesPlayed = attributes.GetProperty("numberOfGames").GetInt16()
                            };
                            result.GlobalStats = global;
                            break;
                        case "clan":
                            var clan = new FAFClan
                            {
                                Name = attributes.GetProperty("name").GetString(),
                                Size = element.GetProperty("relationships")
                                    .GetProperty("memberships")
                                    .GetProperty("data")
                                    .GetArrayLength(),
                                URL = attributes.GetProperty("websiteUrl").GetString()
                            };
                            result.Clan = clan;
                            break;
                        case "nameRecord":
                            var name = attributes.GetProperty("name").GetString();
                            if (name is not null)
                                result.OldNames.Add(name);
                            break;
                    }
                }
            }

            

            return result;
        }
        */
    }
}