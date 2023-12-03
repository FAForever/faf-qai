using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Player
{
    public interface IFetchPlayerStatsOperation
    {
        Task<FetchPlayerStatsResult?> FetchPlayer(string username);
        Task<LeaderboardRatingJournal[]> FetchRatingHistory(string username, Constants.FafLeaderboard leaderboard);
    }
}