#nullable disable warnings
using System;
using System.Collections.Generic;

namespace Faforever.Qai.Core.Operations.FafApi
{
	public enum Faction : byte
	{
		Aeon = 1,
		Cybran = 2,
		Uef = 3,
		Seraphim = 4,
		Random = 5,
		Nomad = 6
	}

	public class FeaturedMod
	{
		public int Id { get; set; }
		public bool? AllowOverride { get; set; }
		public string BireusUrl { get; set; }
		public string DeploymentWebhook { get; set; }
		public string Description { get; set; }
		public string DisplayName { get; set; }
		public string FileExtension { get; set; }
		public string GitBranch { get; set; }
		public string GitUrl { get; set; }
		public short Order { get; set; }
		public string TechnicalName { get; set; }
		public bool Visible { get; set; }
	}

	public class MapVersion
	{
		public int Id { get; set; }
		public DateTime CreateTime { get; set; }
		public string Description { get; set; }
		public string DownloadUrl { get; set; }
		public string Filename { get; set; }
		public string FolderName { get; set; }
		public short Height { get; set; }
		public bool Hidden { get; set; }
		public byte MaxPlayers { get; set; }
		public bool Ranked { get; set; }
		public string ThumbnailUrlLarge { get; set; }
		public string ThumbnailUrlSmall { get; set; }
		public DateTime UpdateTime { get; set; }
		public short Version { get; set; }
		public short Width { get; set; }

		public string Size => $"{Width}x{Height}";

		public Map Map { get; set; }
	}

	public class Map
	{
		public int Id { get; set; }
		public double AverageReviewScore { get; set; }
		public string BattleType { get; set; }
		public DateTime CreateTime { get; set; }
		public string DisplayName { get; set; }
		public string MapType { get; set; }
		public int NumberOfReviews { get; set; }
		public DateTime UpdateTime { get; set; }

		public MapStatistics Statistics { get; set; }
	}

	public class MapStatistics
	{
		public int Id { get; set; }
		public int Downloads { get; set; }
		public int Draws { get; set; }
		public int Plays { get; set; }
	}

	public class PlayerStats
	{
		public int Id { get; set; }
		public Player Player { get; set; }
		public decimal? BeforeMean { get; set; }
		public decimal? BeforeDeviation { get; set; }
		public decimal? AfterMean { get; set; }
		public decimal? AfterDeviation { get; set; }
		public bool Ai { get; set; }
		public byte Color { get; set; }
		public byte Faction { get; set; }
		public sbyte Score { get; set; }
		public DateTime? ScoreTime { get; set; }
		public byte StartSpot { get; set; }
		public sbyte Team { get; set; }

		public int? BeforeRating => GetRating(BeforeMean, BeforeDeviation);
		public int? AfterRating => GetRating(AfterMean, AfterDeviation);

		public string FactionName
		{
			get
			{
				if (!Enum.IsDefined(typeof(Faction), Faction))
					return "Unknown";

				var name = Enum.GetName(typeof(Faction), Faction);

				if (name == "Uef")
					return "UEF";

				return name;
			}
		}

		private static int? GetRating(decimal? mean, decimal? dev)
		{
			if (mean == null || dev == null)
				return null;


			return (int)Math.Round(mean.Value - dev.Value * 3);
		}
	}

	public class Game
	{
		public int Id { get; set; }
		public DateTime? EndTime { get; set; }
		public Player Host { get; set; }
		public MapVersion MapVersion { get; set; }
		public FeaturedMod FeaturedMod { get; set; }
		public string Name { get; set; }
		public string ReplayUrl { get; set; }
		public DateTime StartTime { get; set; }
		public string Validity { get; set; }
		public string VictoryCondition { get; set; }
		public List<PlayerStats> PlayerStats { get; set; }
		public TimeSpan? Duration => EndTime != null ? EndTime - StartTime : null;
	}

	public class Player
	{
		public int Id { get; set; }
		public DateTime CreatedTime { get; set; }
		public string Login { get; set; }
		public DateTime UpdateTime { get; set; }
		public string UserAgent { get; set; }
		public List<NameRecord> Names { get; set; }
	}

	public class NameRecord
	{
		public int Id { get; set; }
		public DateTime ChangeTime { get; set; }
		public string Name { get; set; }
	}
}