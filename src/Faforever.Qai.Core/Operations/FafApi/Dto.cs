#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;

namespace Faforever.Qai.Core.Operations.FafApi
{
    public enum Faction : byte
    {
        Uef = 1,
        Aeon = 2,
        Cybran = 3,
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
        public Uri DownloadUrl { get; set; }
        public string Filename { get; set; }
        public string FolderName { get; set; }
        public short Height { get; set; }
        public bool Hidden { get; set; }
        public byte MaxPlayers { get; set; }
        public bool Ranked { get; set; }
        public Uri ThumbnailUrlLarge { get; set; }
        public Uri ThumbnailUrlSmall { get; set; }
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
        public MapVersion LatestVersion { get; set; }
        public MapStatistics Statistics { get; set; }
        public Player Author { get; set; }
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
        public MapVersion? MapVersion { get; set; }
        public FeaturedMod FeaturedMod { get; set; }
        public string Name { get; set; }
        public long? ReplayTicks { get; set; }
        public string ReplayUrl { get; set; }
        public DateTime StartTime { get; set; }
        public string Validity { get; set; }
        public string VictoryCondition { get; set; }
        public List<PlayerStats> PlayerStats { get; set; }
        public TimeSpan GameDuration => TimeSpan.FromSeconds((ReplayTicks ?? 0) / 10d);
        public TimeSpan? RealDuration => EndTime != null ? EndTime - StartTime : null;

        private double? _averageRating;
        public double AverageRating()
        {
            if (_averageRating == null)
                _averageRating = PlayerStats.Where(p => p.BeforeMean != null && p.BeforeDeviation != null).Average(p => p.BeforeRating);

            return _averageRating ?? 0.0d;
        }
    }

    public class PlayerExtended : Player
    {
        public List<ClanMembership> ClanMembership { get; set; } = new List<ClanMembership>();
    }

    public class Player
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Login { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UserAgent { get; set; }
        public List<NameRecord> Names { get; set; } = new List<NameRecord>();
    }

    public class NameRecord
    {
        public int Id { get; set; }
        public DateTime ChangeTime { get; set; }
        public string Name { get; set; }
    }

    public class MapPool
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Name { get; set; }

        public MatchmakerQueueMapPool MatchmakerQueueMapPool { get; set; }
        public List<MapPoolAssignments> MapPoolAssignments { get; set; }
        public List<MapVersion> MapVersions { get; set; }
    }

    public class MapPoolAssignments
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Spawns { get; set; }
        public int Size { get; set; }
        public string Version { get; set; }
    }

    public class MatchmakerQueueMapPool
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }

        public MatchmakerQueue MatchmakerQueue { get; set; }
        public MapPool MapPool { get; set; }

        public string Name => $"{this.MatchmakerQueue.TechnicalName} (rating: {this.MinRating}-{this.MaxRating})";
    }

    /*
     * "createTime": "2021-12-12T20:21:21Z",
"nameKey": "matchmaker_queue.tmm4v4_share_until_death.name",
"params": "{\"GameOptions\":{\"Share\":\"ShareUntilDeath\"}}",
"teamSize": 4,
"technicalName": "tmm4v4_share_until_death",
"updateTime": "2022-06-26T20:16:15Z"*/
    public class MatchmakerQueue
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string NameKey { get; set; }
        public string Params { get; set; }
        public int TeamSize { get; set; }
        public string TechnicalName { get; set; }

        public MapPool MapPool { get; set; }
    }

    public class LeaderboardRating
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public decimal? Deviation { get; set; }
        public decimal? Mean { get; set; }
        public decimal? Rating { get; set; }
        public int TotalGames { get; set; }
        public int WonGames { get; set; }

        public Leaderboard Leaderboard { get; set; }
        public PlayerExtended Player { get; set; }
    }

    public class LeaderboardRatingJournal
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public decimal? DeviationBefore { get; set; }
        public decimal? MeanBefore { get; set; }
        public decimal? DeviationAfter { get; set; }
        public decimal? MeanAfter { get; set; }

        public decimal BeforeRating => (MeanBefore ?? 0) - (DeviationBefore ?? 0) * 3;
        public decimal AfterRating => (MeanAfter ?? 0) - (DeviationAfter ?? 0) * 3;

        public Leaderboard Leaderboard { get; set; }
        public PlayerStats PlayerStats { get; set; }

    }

    public class Leaderboard
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string DescriptionKey { get; set; }
        public string NameKey { get; set; }
        public string TechnicalName { get; set; }
    }

    public class Clan
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string TagColor { get; set; }
        public string WebsiteUrl { get; set; }
        [Obsolete("Use WebsiteUrl instead.")]
        public string URL => WebsiteUrl;
        public bool RequiresInvitation { get; set; }
        public Player Founder { get; set; }
        public Player Leader { get; set; }
        public List<ClanMembership> Memberships { get; set; } = new List<ClanMembership>();
    }

    public class ClanMembership
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public Clan Clan { get; set; }
        public Player Player { get; set; }
    }
}