using System;
using System.Collections.Generic;

namespace Faforever.Qai.Core.Models
{
	public class FetchPlayerStatsResult
	{
		public string Name { get; set; } = default!;
		public string Id { get; set; } = default!;
		public GameStatistics? LadderStats { get; set; } = null;
		public GameStatistics? GlobalStats { get; set; } = null;
		public FAFClan? Clan { get; set; } = null;
		public List<string> OldNames { get; set; } = new List<string>();
		public ReplayData? ReplayData { get; set; } = null;
	}

	public struct GameStatistics
	{
		public short Ranking { get; set; }
		public decimal Rating { get; set; }
		public short GamesPlayed { get; set; }
	}

	public class FAFClan
	{
		public string? Name { get; set; }
		public int? Size { get; set; }
		public string? URL { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string? Description { get; set; }
		public string? Tag { get; set; }
		public long? Id { get; set; }
	}

	public struct ReplayData
	{
		public int? Team { get; set; }
		public int? Score { get; set; }
		public string? Faction { get; set; }
	}
}