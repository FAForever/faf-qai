using System.Net.Http;

namespace Faforever.Qai.Core.Operations.Clients
{
	public class UnitClient
	{
		public HttpClient Client { get; init; }

		public UnitClient(HttpClient client)
		{
			this.Client = client;
		}
	}
}
