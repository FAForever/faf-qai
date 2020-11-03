using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Faforever.Qai.Core.Structures.Configurations
{
	public class DiscordGuildConfiguration
	{
		[Key]
		public ulong GuildId { get; set; }
		public string Prefix { get; set; }

		// Ignore the value for UserBlacklist, we dont need it.
		public HashSet<ulong> UserBlacklist { get; set; }

		public ConcurrentDictionary<ulong, string> FafLinks { get; set; }
		public ConcurrentDictionary<string, string> Records { get; set; }  

		public DiscordGuildConfiguration() : this(0, "") { } // used by EFcore or simillar processes or creating blank templates.

		public DiscordGuildConfiguration(ulong guildId, string prefix)
		{
			this.GuildId = guildId;
			this.Prefix = prefix;
			this.UserBlacklist = new HashSet<ulong>();
			this.FafLinks = new ConcurrentDictionary<ulong, string>();
			this.Records = new ConcurrentDictionary<string, string>();
		}
	}
}
