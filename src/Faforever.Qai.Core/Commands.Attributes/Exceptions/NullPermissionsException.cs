using System;
using System.Collections.Generic;
using System.Text;

namespace Faforever.Qai.Core.Commands.Attributes.Exceptions
{
	[Serializable]
	public class NullPermissionsException : Exception
	{
		public NullPermissionsException() { }
		public NullPermissionsException(string message) : base(message) { }
		public NullPermissionsException(string message, Exception inner) : base(message, inner) { }
		protected NullPermissionsException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
