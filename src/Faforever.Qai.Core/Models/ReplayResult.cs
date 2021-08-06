using System;
using System.Collections.Generic;

namespace Faforever.Qai.Core.Models
{
	public class ReplayResult
	{
		public string? Title { get; set; }
		public Uri? ReplayUri { get; set; }
		public long Id { get; set; }
		public DateTime? StartTime { get; set; }
		public string? VictoryConditions { get; set; }
		public string? Validity { get; set; }
		public MapResult? MapInfo { get; set; }
		public List<FetchPlayerStatsResult>? PlayerData { get; set; }
		public bool Ranked1v1 { get; set; }
	}
}
