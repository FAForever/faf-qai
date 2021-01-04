using System.Collections.Generic;

using Newtonsoft.Json;

namespace Faforever.Qai.Core.Structures.Configurations
{
	public class BotFunConfiguration
	{
		[JsonProperty("eightball_phrases")]
		public List<string> EightballPhrases { get; set; }
		[JsonProperty("default_taunts")]
		public List<string> Taunts { get; set; }
		[JsonProperty("spam_protection_taunts")]
		public List<string> SpamProtectionTaunts { get; set; }
		[JsonProperty("kick_taunts")]
		public List<string> KickTaunts { get; set; }
	}
}
