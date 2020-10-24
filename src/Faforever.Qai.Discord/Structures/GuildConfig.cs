using System;
using System.Collections.Generic;
using System.Text;

namespace Faforever.Qai.Discord.Structures
{
	public class GuildConfig
	{
		public ulong GuildId { get; set; }
		public string Prefix { get; set; }

		public HashSet<ulong> UserBlacklist { get; set; }

		public GuildConfig() { } // used by EFcore or simillar processes or creating blank templates.

		public GuildConfig(ulong guildId)
		{
			this.GuildId = guildId;
			this.UserBlacklist = new HashSet<ulong>();
		}
	}
}
