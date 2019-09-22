using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Mozilla.IoT.WebThing.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class DescriptionException : Exception
    {
        public DescriptionException()
        {
        }

        protected DescriptionException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        public DescriptionException(string message) 
            : base(message)
        {
        }

        public DescriptionException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
