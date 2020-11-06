using System;
using System.Runtime.Serialization;

namespace Faforever.Qai.Core.Commands.Context.Exceptions
{
	public class NullPrefixException : Exception
	{
		public NullPrefixException() : base() { }
		public NullPrefixException(string? message) : base(message) { }
		public NullPrefixException(string? message, Exception? innerException) : base(message, innerException) { }
		public NullPrefixException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
