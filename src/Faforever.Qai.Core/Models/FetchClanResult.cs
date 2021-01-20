using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Models
{
	public class FetchClanResult
	{
		public FAFClan Clan { get; set; }
		public List<ShortPlayerData> Members { get; set; } = new();
	}

	public struct ShortPlayerData
	{
		public string? Username { get; set; }
		public DateTime? JoinDate { get; set; }
	}
}
