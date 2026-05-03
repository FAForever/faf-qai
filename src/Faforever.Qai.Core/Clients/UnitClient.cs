using System.Net.Http;

namespace Faforever.Qai.Core.Clients
{
    public class UnitClient(HttpClient client)
    {
        public HttpClient Client { get; init; } = client;
    }
}
