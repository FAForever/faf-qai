using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Services
{
    public enum FafMod
    {
        Faf = 0,
        Ladder1v1 = 6
    }

    public class GameService
    {
        private readonly FafApiClient _api;

        public GameService(FafApiClient api)
        {
            this._api = api;
        }

        public async Task<Game?> FetchGame(long gameId)
        {
            var query = new ApiQuery<Game>()
                .Where("id", gameId)
                .Limit(1);

            var games = await _api.GetAsync(query);

            return games.FirstOrDefault();
        }

        public async Task<Game?> FetchLastGame(string username)
        {
            var query = new ApiQuery<Game>()
                .Where("playerStats.player.login", username)
                .Sort("-startTime")
                .Limit(1);

            var games = await _api.GetAsync(query);

            return games.FirstOrDefault();
        }

        public async Task<IReadOnlyList<Game>> FetchTopRatedGames(string? mapName = null, FafMod? mod = null)
        {
            var query = new ApiQuery<Game>()
                .Where("validity", "VALID")
                .Where("replayAvailable", "true")
                .Sort("-startTime")
                .Limit(200);

            if (mapName == null)
            {
                if (mod == FafMod.Faf)
                    query.Where("playerStats.beforeMean", WhereOp.GreaterThan, "2000");

                query.Where("startTime", WhereOp.GreaterThan, DateTime.Now.AddDays(-7));
            }
            else
            {
                query.Where("mapVersion.map.displayName", $"*{mapName}*");
                query.Where("startTime", WhereOp.GreaterThan, DateTime.Now.AddDays(-30));
            }

            if (mod != null)
                query.Where("featuredMod.id", (int)mod);

            
            IEnumerable<Game> games = await _api.GetAsync(query);

            return games.Where(g => g.PlayerStats.Count > 1).OrderByDescending(g => g.AverageRating()).Take(3).ToList();
        }
    }
}
