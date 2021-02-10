using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Clients;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Faforever.Qai.Core.Operations.Content
{
	public class FetchTwitchStreamsOperation : IFetchTwitchStreamsOperation
	{
		private readonly TwitchClient _client;
		public const int GameId = 16553;

		public FetchTwitchStreamsOperation(TwitchClient client)
		{
			_client = client;
		}

		public async Task<TwitchStreamsResult?> GetTwitchStreamsAsync()
		{
			var resJson = await _client.GetCurrentStreams(GameId);

			if (resJson is null) return null;

			var res = JsonConvert.DeserializeObject<TwitchStreamsResult>(resJson);

			return res;
		}
	}
}
