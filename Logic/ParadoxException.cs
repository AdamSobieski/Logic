using System;
using System.Runtime.Serialization;

namespace Logic
{
    [Serializable]
    public sealed class ParadoxException : Exception
    {
        public ParadoxException()
            : base() { }

        public ParadoxException(string message)
            : base(message) { }

        public ParadoxException(string message, Exception innerException)
            : base(message, innerException) { }

        public ParadoxException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
