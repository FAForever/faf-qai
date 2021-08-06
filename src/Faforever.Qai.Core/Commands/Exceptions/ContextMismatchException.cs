using System;

namespace Faforever.Qai.Core.Commands.Exceptions
{
	class ContextMismatchException : Exception
	{
		public ContextMismatchException(string? message) : base(message)
		{
		}
	}
}
