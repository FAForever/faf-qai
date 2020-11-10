using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Player
{
	[ExcludeFromCodeCoverage]
	public class ApiFetchPlayerStatsOperation : IFetchPlayerStatsOperation
	{
		private readonly HttpClient _client;

		public ApiFetchPlayerStatsOperation(HttpClient client)
		{
			client.BaseAddress = new Uri("https://api.faforever.com/");
			_client = client;
		}

		public async Task<FetchPlayerStatsResult> FetchPlayer(string username)
		{
			using Stream? stream =
				await _client.GetStreamAsync(
					$"/data/player?include=globalRating,ladder1v1Rating,names&filter=login=={username}");
			using JsonDocument json = await JsonDocument.ParseAsync(stream);
			JsonElement includedElement = json.RootElement.GetProperty("included");
			FetchPlayerStatsResult result = new FetchPlayerStatsResult
			{
				Name = username
			};
			foreach (JsonElement element in includedElement.EnumerateArray())
			{
				var typeElement = element.GetProperty("type");
				var attributes = element.GetProperty("attributes");
				switch (typeElement.GetString())
				{
					case "ladder1v1Rating":
						result.Rating1v1 = attributes.GetProperty("rating").GetDecimal();
						result.Ranking1v1 = attributes.GetProperty("ranking").GetInt16();

						break;
					case "globalRating":
						result.RatingGlobal = attributes.GetProperty("rating").GetDecimal();
						result.RankingGlobal = attributes.GetProperty("ranking").GetInt16();
						break;
				}
			}

			return result;
		}
	}
}