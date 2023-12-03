using System.Net.Http;

namespace Faforever.Qai.Core.Clients
{
    public class UnitClient
    {
        public HttpClient Client { get; init; }

        public UnitClient(HttpClient client)
        {
            Client = client;
        }
    }
}
