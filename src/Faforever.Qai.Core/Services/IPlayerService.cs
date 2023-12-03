using System.Threading.Tasks;
using Faforever.Qai.Core.Constants;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Services
{
    public interface IPlayerService
    {
        Task<FetchPlayerStatsResult?> FetchPlayerStats(string username);
        Task<FindPlayerResult> FindPlayer(string searchTerm);
        Task<byte[]> GenerateRatingChart(string username, FafLeaderboard leaderboard);
        Task<LeaderboardRatingJournal[]> GetRatingHistory(string username, FafLeaderboard leaderboard);
        Task<LastSeenPlayerResult?> LastSeenPlayer(string username);
    }
}