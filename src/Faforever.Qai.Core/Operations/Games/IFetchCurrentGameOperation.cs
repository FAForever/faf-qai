using System.Threading.Tasks;
using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Games
{
    public interface IFetchCurrentGameOperation
    {
        Task<CurrentGameResult?> FetchCurrentGame(string fafUsername);
        Task<CurrentGameResult?> FetchCurrentGameById(int fafId);
    }
}