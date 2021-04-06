using System;
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
		public ulong DisocrdId { get; internal set; }
		public int FafId { get; internal set; }
		public string? FafUsername { get; internal set; }
		public string? DiscordUsername { get; internal set; }
	}
}
