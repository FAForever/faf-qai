using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Clan {
	public class ApiFetchClanOperation : IFetchClanOperation {
		private readonly HttpClient _client;

		public ApiFetchClanOperation(HttpClient client) {
			_client = client;
		}

		public async Task<FetchClanResult> FetchClan(string tag) {
			using Stream? stream =
				await _client.GetStreamAsync(
					$"https://api.faforever.com/data/clan?include=memberships.player&fields[player]=login&fields[clanMembership]=createTime,player&fields[clan]=name,description,websiteUrl,createTime,tag,leader&filter=tag=='{tag}'");

			using JsonDocument json = await JsonDocument.ParseAsync(stream);

			JsonElement dataElement = json.RootElement.GetProperty("data");

			string name = string.Empty;
			string description = string.Empty;
			string url = string.Empty;

			foreach (JsonElement clan in dataElement.EnumerateArray()) {
				if (!clan.TryGetProperty("type", out JsonElement typeElement) || typeElement.GetString() != "clan")
					continue;

				var attributes = clan.GetProperty("attributes");

				name = attributes.GetProperty("name").GetString() ?? string.Empty;
				description = attributes.GetProperty("description").GetString() ?? string.Empty;
				tag = attributes.GetProperty("tag").GetString() ?? string.Empty;
				url = attributes.GetProperty("websiteUrl").GetString() ?? string.Empty;
			}


			JsonElement includedElement = json.RootElement.GetProperty("included");

			List<string> playerNames = new();
			foreach (JsonElement element in includedElement.EnumerateArray()) {
				var typeElement = element.GetProperty("type");
				var attributes = element.GetProperty("attributes");
				switch (typeElement.GetString()) {
					case "player":
						string? loginName = attributes.GetProperty("login").GetString();

						if (loginName is not null)
							playerNames.Add(loginName);
						break;
				}
			}

			return new FetchClanResult {
				Name = name,
				Description = description,
				Tag = tag,
				Url = url,
				PlayerNames = playerNames
			};
		}
	}
}