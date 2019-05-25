using System;
using System.Runtime.Serialization;

namespace Mozzila.IoT.WebThing.Exceptions
{
    [Serializable]
    public class PropertyException : Exception
    {
        public PropertyException()
        {
        }

        protected PropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
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
