using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Replays
{
    public class ApiFetchReplayOperation(FafApiClient api) : IFetchReplayOperation
    {
        public async Task<Game?> FetchLastReplayAsync(string username)
        {
            var query = new ApiQuery<Game>()
                .Where("playerStats.player.login", username)
                .Limit(1);
            var games = await api.GetAsync(query);
            var game = games.FirstOrDefault();
            if (game is null) return null;

            return game;
        }

        public async Task<Game?> FetchReplayAsync(long replayId)
        {
            var query = new ApiQuery<Game>()
                .Where("id", replayId)
                .Limit(1);
            var games = await api.GetAsync(query);
            var game = games.FirstOrDefault();
            if (game is null) return null;

            return game;
        }
    }
}
