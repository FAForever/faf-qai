using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Faforever.Qai.Core.Structures
{
	public class RelayConfiguration
	{
		[Key]
		public ulong DiscordServer { get; set; }
		[NotMapped]
		public ConcurrentDictionary<string, string> IRCToDiscordLinks { get; set; }
		[NotMapped]
		public ConcurrentDictionary<ulong, string> DiscordToIRCLinks { get; set; }

		/* Discord ---------> IRC
		 * MSG R -> Find IRC Channel				-> Send Message from User.
		 *		 -> Find All other Discord Channels ^^
		 *		 
		 * -- Ensure Message is not from Dostya (or another bot).
		 * 
		 * IRC ---------> Discord
		 * MSG R -> Find All Discord Channels -> Send Message from User.
		 * 
		 * -- Relay ALL messages except for messages from self. (Other IRC bot commands included).
		 * - OR -
		 * -- Seprate IRC connection for relay, so QAI IRC command resulst are still sent to Discord (Current functionality).
		 */
	}
}
