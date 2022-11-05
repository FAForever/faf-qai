using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Clients;

namespace Faforever.Qai.Core.Operations.Player
{
    [ExcludeFromCodeCoverage]
    public class ApiFetchPlayerStatsOperation : IFetchPlayerStatsOperation
    {
        private readonly ApiHttpClient _api;

        public ApiFetchPlayerStatsOperation(ApiHttpClient api)
        {
            this._api = api;
        }

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

            JsonElement includedElement = json.RootElement.GetProperty("included");

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
                        if(name is not null)
                            result.OldNames.Add(name);
                        break;
                }
            }

            return result;
        }
    }
}