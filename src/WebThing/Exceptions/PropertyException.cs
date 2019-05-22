using System;
using System.Runtime.Serialization;
using WebThing.Annotations;

namespace WebThing.Exceptions
{
    [Serializable]
    public class PropertyException : Exception
    {
        public PropertyException()
        {
        }

        protected PropertyException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PropertyException(string message) : base(message)
        {
        }

        public PropertyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
