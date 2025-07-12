using System;

namespace Faforever.Qai.Core.Commands.Authorization.Exceptions
{
    public class NullPermissionsException : Exception
    {
        public NullPermissionsException() { }
        public NullPermissionsException(string message) : base(message) { }
        public NullPermissionsException(string message, Exception inner) : base(message, inner) { }
    }
}
