using System.Threading.Tasks;
using Faforever.Qai.Core.Clients;
using Faforever.Qai.Core.Models;

using Newtonsoft.Json;

namespace Faforever.Qai.Core.Operations.Content
{
    public class FetchTwitchStreamsOperation(TwitchClient client) : IFetchTwitchStreamsOperation
    {
        public const int GameId = 16553;

        public async Task<TwitchStreamsResult?> GetTwitchStreamsAsync()
        {
            var resJson = await client.GetCurrentStreams(GameId);

            if (resJson is null) return null;

            var res = JsonConvert.DeserializeObject<TwitchStreamsResult>(resJson);

            return res;
        }
    }
}
