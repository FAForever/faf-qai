using System;
using System.Collections.Generic;
using System.Text;

namespace Faforever.Qai.Discord.Structures
{
	public class GuildConfig
	{
		public ulong GuildId { get; set; }

		public HashSet<ulong> UserBlacklist { get; set; }

		public GuildConfig(ulong guildId)
		{
			this.GuildId = guildId;
			this.UserBlacklist = new HashSet<ulong>();
		}
	}
}
