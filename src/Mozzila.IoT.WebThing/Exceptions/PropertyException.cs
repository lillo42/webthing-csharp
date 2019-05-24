using System;
using System.Runtime.Serialization;
using Mozzila.IoT.WebThing.Annotations;

namespace Mozzila.IoT.WebThing.Exceptions
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
