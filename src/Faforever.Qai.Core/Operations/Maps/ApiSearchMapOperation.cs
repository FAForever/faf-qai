using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Clients;

using Newtonsoft.Json;

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
			var res = MapRequest.FromJson(data);

			if (res.Data.Length <= 0) return null;

			var map = res.Data[0];
			var included = res.Included.FirstOrDefault(x => x.Id == map.Relationships.LatestVersion.Data.Dat?.Id);
			var player = res.Included.FirstOrDefault(x => x.Type == TypeEnum.Player);

			if (included is null) return null;

			var size = GetMapSize(included.Attributes.Width ?? 0);

			return new()
			{
				Title = map.Attributes.DisplayName,
				CreatedAt = map.Attributes.CreateTime.UtcDateTime,
				Id = map.Id,
				Ranked = included.Attributes.Ranked ?? false,
				DownlaadUrl = included.Attributes.DownloadUrl,
				PreviewUrl = included.Attributes.ThumbnailUrlLarge,
				Description = included.Attributes.Description.RemoveBadContent(),
				MaxPlayers = included.Attributes.MaxPlayers ?? 0,
				Author = player?.Attributes.Login ?? "Unkown Author",
				Size = $"{size}x{size} km"
			};
		}

		public static long GetMapSize(long i)
		{
			var b = (i / 5) / 10;

			var digits = Math.Floor(Math.Log10(b) + 1);
			var nearest = (int)Math.Pow(10, digits - 1);

			return (b + 5 * nearest / 10) / nearest * nearest;
		}
	}
}
