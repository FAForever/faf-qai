using System;
using System.Collections.Generic;

namespace Faforever.Qai.Core.Models
{
	public class FetchClanResult
	{
		public FAFClan Clan { get; set; } = default!;
		public List<ShortPlayerData> Members { get; set; } = new();
	}

	public struct ShortPlayerData
	{
		public string? Username { get; set; }
		public DateTime? JoinDate { get; set; }
	}
}
