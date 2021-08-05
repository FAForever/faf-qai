using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Structures.Link
{

	public class AccountLink
	{
		[Key]
		public ulong DiscordId { get; set; }
		public int FafId { get; set; }
		public string? FafUsername { get; set; }
		public string? DiscordUsername { get; set; }
	}
}
