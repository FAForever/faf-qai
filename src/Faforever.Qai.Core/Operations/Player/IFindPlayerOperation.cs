using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Player
{
    public interface IFindPlayerOperation
    {
        Task<FindPlayerResult> FindPlayer(string searchTerm);
    }
}