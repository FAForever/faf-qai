using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Commands.Exceptions
{
	class ContextMismatchException : Exception
	{
		public ContextMismatchException(string? message) : base(message)
		{
		}
	}
}
