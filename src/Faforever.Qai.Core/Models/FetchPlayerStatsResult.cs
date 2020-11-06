namespace Faforever.Qai.Core.Models
{
	public class FetchPlayerStatsResult
	{
		public short Ranking1v1 { get; set; }
		public decimal Rating1v1 { get; set; }
		public string Name { get; set; }
		public decimal RatingGlobal { get; set; }
		public short RankingGlobal { get; set; }
	}
}