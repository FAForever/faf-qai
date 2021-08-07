using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Services
{
    public interface IPlayerService
    {
        Task<FetchPlayerStatsResult> FetchPlayerStats(string username);
        Task<FindPlayerResult> FindPlayer(string searchTerm);
        Task<LastSeenPlayerResult?> LastSeenPlayer(string username);
    }
}