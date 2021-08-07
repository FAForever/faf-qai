using System.Net.Http;

namespace Faforever.Qai.Core.Operations.Clients
{
    public class ApiHttpClient
    {
        public HttpClient Client { get; init; }

        public ApiHttpClient(HttpClient client)
        {
            this.Client = client;
        }
    }
}
