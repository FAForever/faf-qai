using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrcDotNet;

namespace Faforever.Qai.Core.Commands.Arguments
{
	public class IrcUserCapsule : IBotUserCapsule
	{
		public IrcUser User { get; private set; }
		public IrcUserCapsule(IrcUser u)
		{
			User = u;
		}
	}
}
