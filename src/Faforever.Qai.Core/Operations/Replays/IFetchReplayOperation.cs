using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Replays
{
    public interface IFetchReplayOperation
    {
        public Task<ReplayResult?> FetchReplayAsync(long replayId);
        public Task<ReplayResult?> FetchLastReplayAsync(string username);
    }
}
