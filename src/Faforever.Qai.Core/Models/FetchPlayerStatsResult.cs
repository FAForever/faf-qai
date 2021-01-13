using System.Collections.Generic;

namespace Faforever.Qai.Core.Models
{
	public class FetchPlayerStatsResult
	{
		public string Name { get; set; }
		public string Id { get; set; }
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

	public struct FAFClan
	{
		public string Name { get; set; }
		public int Size { get; set; }
		public string URL { get; set; }
	}

	public struct ReplayData
	{
		public int? Team { get; set; }
		public int? Score { get; set; }
		public string? Faction { get; set; }
	}
}