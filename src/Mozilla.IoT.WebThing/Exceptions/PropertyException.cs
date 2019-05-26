using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Mozilla.IoT.WebThing.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
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
