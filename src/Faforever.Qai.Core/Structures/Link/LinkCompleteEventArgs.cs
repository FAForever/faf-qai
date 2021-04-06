using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Structures.Link
{
	public class LinkCompleteEventArgs
	{
		public bool Complete { get; internal set; }
		public ulong Guild { get; internal set; }
		public AccountLink? Link { get; internal set; }
	}
}
