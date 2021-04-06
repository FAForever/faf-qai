using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Structures.Link
{
	public class LinkState
	{
		public string Token { get; init; }
		public ulong DiscordId { get; init; }
		public string DiscordUsername { get; init; }
		public int? FafId { get; init; }
		public string? FafUsername { get; init; }
		public bool LinkReady { get; init; }
		public Timer ExparationTimer { get; init; }

		public LinkState(string token, ulong discordId, string discordUsername, Timer timer)
		{
			Token = token;
			DiscordId = discordId;
			DiscordUsername = discordUsername;
			ExparationTimer = timer;
			LinkReady = false;
		}

		public LinkState(string token, ulong discordId, string discordUsername, int fafId, string fafUsername, Timer timer)
		{
			Token = token;
			DiscordId = discordId;
			DiscordUsername = discordUsername;
			FafId = fafId;
			FafUsername = fafUsername;
			ExparationTimer = timer;
			LinkReady = true;
		}
	}
}
