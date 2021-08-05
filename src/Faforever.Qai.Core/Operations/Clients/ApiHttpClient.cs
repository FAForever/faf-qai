using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
