using System.Net.Http;

namespace Faforever.Qai.Core.Clients
{
    public class ApiHttpClient(HttpClient client)
    {
        public HttpClient Client { get; init; } = client;
    }
}
