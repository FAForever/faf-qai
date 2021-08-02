using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Clients;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Faforever.Qai.Core.Operations.Maps
{
	public class ApiSearchMapOperation : ISearchMapOperation
	{
		private readonly ApiClient _api;

		public ApiSearchMapOperation(ApiClient api)
		{
			_api = api;
		}

		public async Task<MapResult?> GetMapAsync(string map)
		{
			string data =
				await this._api.Client.GetStringAsync(
					$"/data/map?page[size]=1&include=versions,author&filter=displayName==\"{map}\"");

			return ParseStringData(data);
		}

		public async Task<MapResult?> GetMapAsync(int mapId)
		{
			string data =
				await this._api.Client.GetStringAsync(
					$"/data/map?page[size]=1&include=versions,author&filter=id=={mapId}");

			return ParseStringData(data);
		}

		private MapResult? ParseStringData(string data)
		{
			var json = JObject.Parse(data);

			JToken? map = json["data"]?.First;

			if (map is null || map["type"]?.ToString() != "map") return null;

			var revision = map["relationships"]?["latestVersion"]?["data"]?["id"]?.ToObject<long>() ?? 0;

			JToken? included = json["included"]?.FirstOrDefault(x => x["id"]?.ToObject<long>() == revision);

			if (included is null) return null;

			JToken? player = json["included"]?.FirstOrDefault(x => x["type"]?.ToString() == "player");

			var size = included["attributes"]?["height"]?.ToObject<long>().GetMapSize() ?? 0;

			return new()
			{
				Title = map["attributes"]?["displayName"]?.ToString(),
				CreatedAt = map["attributes"]?["createTime"]?.ToObject<DateTime>(),
				Id = map["id"]?.ToObject<long>() ?? 0,
				Ranked = included["attributes"]?["ranked"]?.ToObject<bool>(),
				DownloadUrl = included["attributes"]?["downloadUrl"]?.ToObject<Uri>(),
				PreviewUrl = included["attributes"]?["thumbnailUrlLarge"]?.ToObject<Uri>(),
				Description = included["attributes"]?["description"]?.ToString().RemoveBadContent(),
				MaxPlayers = included["attributes"]?["maxPlayers"]?.ToObject<long>(),
				Author = player?["attributes"]?["login"]?.ToString(),
				Size = $"{size}x{size} km",
				Version = map["attributes"]?["version"]?.ToObject<int>()
			};
		}
	}
}
