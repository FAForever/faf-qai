using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Clients;

using Newtonsoft.Json.Linq;

namespace Faforever.Qai.Core.Operations.Replays
{
	public class ApiFetchReplayOperation : IFetchReplayOperation
	{
		private readonly ApiHttpClient _api;
		private const string baseurl = "/data/game?include=mapVersion,playerStats,mapVersion.map," +
				"playerStats.player,featuredMod,playerStats.player.globalRating," +
				"playerStats.player.ladder1v1Rating,playerStats.player.clanMembership.clan";

		public ApiFetchReplayOperation(ApiHttpClient api)
		{
			_api = api;
		}

		public async Task<ReplayResult?> FetchLastReplayAsync(string username)
		{
			string data = await _api.Client
				.GetStringAsync($"{baseurl}&filter=playerStats.player.login=={username}");

			return ParseRawJson(data);
		}

		public async Task<ReplayResult?> FetchReplayAsync(long replayId)
		{
			string data = await _api.Client
				.GetStringAsync($"{baseurl}&filter=id=={replayId}");

			return ParseRawJson(data);
		}

		private ReplayResult? ParseRawJson(string data)
		{
			var json = JObject.Parse(data);

			var potGames = json["data"]?.Where(x => x["type"]?.ToString() == "game");

			if (potGames is null) return null;

			JToken? game = potGames.LastOrDefault();

			if (game is null) return null;

			JToken? attributes = game["attributes"];
			ReplayResult res;
			if (attributes is not null)
				res = new()
				{
					Title = attributes["name"]?.ToString(),
					Id = game["id"]?.ToObject<long>() ?? 0,
					StartTime = attributes["startTime"]?.ToObject<DateTime>(),
					ReplayUri = attributes["replayUrl"]?.ToObject<Uri>(),
					Validity = attributes["validity"]?.ToString(),
					VictoryConditions = attributes["victoryCondition"]?.ToString(),
				};
			else res = new();

			JToken? map;
			try
			{
				map = json["included"]?.FirstOrDefault(x => x["type"]?.ToString() == "map"
					&& x["relationships"]?["latestVersion"]?["data"]?["id"]?.ToObject<long>()
						== game["relationships"]?["mapVersion"]?["data"]?["id"]?.ToObject<long>());
			}
			catch
			{
				map = null;
			}

			if (map is not null)
			{
				var revision = map["relationships"]?["latestVersion"]?["data"]?["id"]?.ToObject<long>() ?? 0;

				JToken? included = json["included"]?.FirstOrDefault(x => x["id"]?.ToObject<long>() == revision);

				if (included is not null)
				{
					var size = included["attributes"]?["height"]?.ToObject<long>().GetMapSize() ?? 0;

					res.MapInfo = new()
					{
						Title = map["attributes"]?["displayName"]?.ToString(),
						PreviewUrl = included["attributes"]?["thumbnailUrlLarge"]?.ToObject<Uri>(),
						Ranked = included["attributes"]?["ranked"]?.ToObject<bool>(),
						Size = $"{size}x{size} km",
						Version = included["attributes"]?["version"]?.ToObject<int>()
					};
				}
			}

			JToken? feature = json["included"]?.FirstOrDefault(x => x["type"]?.ToString() == "featureMod");

			res.Ranked1v1 = feature?["attributes"]?["technicalName"]?.ToString() == "ladder1v1";

			var playerStatsRaw = game["relationships"]?["playerStats"]?["data"];

			if (playerStatsRaw is null) return res;

			List<JToken>? gamePlayerStats = json["included"]?.Where(x => playerStatsRaw.Any(y => y["id"]?.ToObject<long>() == x["id"]?.ToObject<long>())
				&& x["type"]?.ToString() == "gamePlayerStats")?.ToList();

			if (gamePlayerStats is null) return res;

			List<JToken>? players = json["included"]?.Where(x => 
				gamePlayerStats.Any(y => y["relationships"]?["player"]?["data"]?["id"]?.ToObject<long>() == x["id"]?.ToObject<long>())
				&& x["type"]?.ToString() == "player")?.ToList();
			List<JToken>? playerRaitings = json["included"]?.Where(x => x["type"]?.ToString() == "globalRating")?.ToList();
			List<JToken>? playerRankedRaitings = json["included"]?.Where(x => x["type"]?.ToString() == "ladder1v1Rating")?.ToList();

			if (players is not null)
			{
				res.PlayerData = new List<FetchPlayerStatsResult>();

				foreach (var p in players)
				{
					FetchPlayerStatsResult pData = new()
					{
						Name = p["attributes"]?["login"]?.ToString() ?? "n/a",
						Id = p["id"]?.ToString() ?? "",
					};

					if (gamePlayerStats is not null)
					{
						var gameStats = gamePlayerStats.FirstOrDefault(x => x["relationships"]?["player"]?["data"]?["id"]?.ToString() == pData.Id);

						if (gameStats is not null)
						{
							pData.ReplayData = new()
							{
								Faction = gameStats["attributes"]?["faction"]?.ToObject<int>().GetFaction(),
								Score = gameStats["attributes"]?["score"]?.ToObject<int>(),
								Team = gameStats["attributes"]?["team"]?.ToObject<int>()
							};
						}
					}

					if (playerRankedRaitings is not null)
					{
						var rankStats = playerRankedRaitings.FirstOrDefault(x => x["relationships"]?["player"]?["data"]?["id"]?.ToString() == pData.Id);

						if (rankStats is not null)
						{
							pData.GlobalStats = new()
							{
								GamesPlayed = rankStats["attributes"]?["numberOfGames"]?.ToObject<short>() ?? 0,
								Ranking = rankStats["attributes"]?["ranking"]?.ToObject<short>() ?? 0,
								Rating = rankStats["attributes"]?["ranking"]?.ToObject<short>() ?? 0
							};
						}
					}

					if (playerRaitings is not null)
					{
						var rankStats = playerRaitings.FirstOrDefault(x => x["relationships"]?["player"]?["data"]?["id"]?.ToString() == pData.Id);

						if (rankStats is not null)
						{
							pData.LadderStats = new()
							{
								GamesPlayed = rankStats["attributes"]?["numberOfGames"]?.ToObject<short>() ?? 0,
								Ranking = rankStats["attributes"]?["ranking"]?.ToObject<short>() ?? 0,
								Rating = rankStats["attributes"]?["ranking"]?.ToObject<short>() ?? 0
							};
						}
					}

					res.PlayerData.Add(pData);
				}
			}

			return res;
		}
	}
}
