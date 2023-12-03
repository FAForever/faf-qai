using System.Threading.Tasks;
using Faforever.Qai.Core.Operations.FafApi;

namespace Faforever.Qai.Core.Operations.Replays
{
    public interface IFetchReplayOperation
    {
        public Task<Game?> FetchReplayAsync(long replayId);
        public Task<Game?> FetchLastReplayAsync(string username);
    }
}
