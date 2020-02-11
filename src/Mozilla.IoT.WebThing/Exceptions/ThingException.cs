using System;
using System.Runtime.Serialization;

namespace Mozilla.IoT.WebThing.Exceptions
{
    public class ThingException : Exception
    {
        public ThingException()
        {
        }

        protected ThingException(SerializationInfo? info, StreamingContext context) 
            : base(info, context)
        {
        }

        public ThingException(string? message) 
            : base(message)
        {
        }

        public ThingException(string? message, Exception? innerException) 
            : base(message, innerException)
        {
        }
    }
}
