using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

using Faforever.Qai.Core.Structures.Webhooks;

namespace Faforever.Qai.Core.Database.Entities
{
    public class RelayConfiguration
    {
        // feel free to ingnore my notes -- Soyvolon
        [Key]
        public ulong DiscordServer { get; set; }
        // irc chan, discord chan
        public ConcurrentDictionary<ulong, string> DiscordToIRCLinks { get; set; }
        // irc chan, webhook(id, token, discord chan)
        public ConcurrentDictionary<string, DiscordWebhookData> Webhooks { get; set; }

        public RelayConfiguration()
        {
            DiscordToIRCLinks = new ConcurrentDictionary<ulong, string>();
            Webhooks = new ConcurrentDictionary<string, DiscordWebhookData>();
        }

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
