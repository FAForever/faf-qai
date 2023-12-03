using System.Net.Http;

namespace Faforever.Qai.Core.Clients
{
    public class ApiHttpClient
    {
        public HttpClient Client { get; init; }

        public ApiHttpClient(HttpClient client)
        {
            Client = client;
        }
    }
}
