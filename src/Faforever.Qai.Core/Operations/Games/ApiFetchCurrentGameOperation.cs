using System;
using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Games
{
    public class ApiFetchCurrentGameOperation : IFetchCurrentGameOperation
    {
        private readonly FafApiClient _api;

        public ApiFetchCurrentGameOperation(FafApiClient api)
        {
            _api = api;
        }

        public async Task<CurrentGameResult?> FetchCurrentGame(string fafUsername)
        {
            var query = new ApiQuery<FafApi.Game>()
                .Where("playerStats.player.login", fafUsername)
                .Sort("-startTime")
                .Include("host,playerStats.player")
                .Limit(1);

            var games = await _api.GetAsync(query);
            var game = games.FirstOrDefault();

            if (game is null || game.EndTime.HasValue)
                return null;

            return ConvertToCurrentGameResult(game);
        }

        public async Task<CurrentGameResult?> FetchCurrentGameById(int fafId)
        {
            var query = new ApiQuery<FafApi.Game>()
                .Where("playerStats.player.id", fafId)
                .Sort("-startTime")
                .Include("host,playerStats.player")
                .Limit(1);

            var games = await _api.GetAsync(query);
            var game = games.FirstOrDefault();

            if (game is null || game.EndTime.HasValue)
                return null;

            return ConvertToCurrentGameResult(game);
        }

        private static CurrentGameResult ConvertToCurrentGameResult(FafApi.Game game)
        {
            var players = game.PlayerStats
                .Where(ps => ps.Player != null)
                .Select(ps => new GamePlayer
                {
                    FafId = ps.Player.Id,
                    Username = ps.Player.Login,
                    Team = (int)ps.Team // Keep original team numbers: -1 = observer, 1+ = teams
                })
                .ToList();

            // Count only actual teams (exclude observers with team -1)
            var actualTeams = players.Where(p => p.Team > 0).ToList();
            var teamCount = actualTeams.Any() ? actualTeams.Max(p => p.Team) : 0;

            return new CurrentGameResult
            {
                GameId = game.Id.ToString(),
                GameName = game.Name ?? "Unknown Game",
                HostUsername = game.Host?.Login ?? "Unknown Host",
                StartTime = game.StartTime,
                Players = players,
                TeamCount = teamCount
            };
        }
    }
}