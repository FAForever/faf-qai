using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Faforever.Qai.Core.Clients;
using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Player
{
    public class ApiFindPlayerOperation(ApiHttpClient api) : IFindPlayerOperation
    {
        public async Task<FindPlayerResult> FindPlayer(string searchTerm)
        {
            using Stream? stream =
                await api.Client.GetStreamAsync(
                    $"/data/player?include=names&filter=login==*{searchTerm}*");
            using JsonDocument json = await JsonDocument.ParseAsync(stream);
            JsonElement dataElement = json.RootElement.GetProperty("data");
            FindPlayerResult result = new FindPlayerResult();
            foreach (JsonElement element in dataElement.EnumerateArray())
            {
                var typeElement = element.GetProperty("type");
                if (typeElement.GetString() != "player")
                    continue;

                var attributes = element.GetProperty("attributes");

                var username = attributes.GetProperty("login").GetString();
                if(username is not null)
                    result.Usernames.Add(username);
            }

            return result;
        }
    }
}