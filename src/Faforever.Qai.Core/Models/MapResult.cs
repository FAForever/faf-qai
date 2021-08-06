using System;

namespace Faforever.Qai.Core.Models
{
	public class MapResult
	{
		public string? Title { get; set; }
		public long Id { get; set; }
		public string? Description { get; set; }
		public Uri? DownloadUrl { get; set; }
		public string? Size { get; set; }
		public long? MaxPlayers { get; set; }
		public bool? Ranked { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? Author { get; set; }
		public Uri? PreviewUrl { get; set; }
		public int? Version { get; set; }

		public MapResult()
		{
			Title = "";
			Description = "";
			Size = "";
			Author = "";
		}
	}
}
