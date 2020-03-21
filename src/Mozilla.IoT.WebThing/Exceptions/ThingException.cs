using System;

namespace Mozilla.IoT.WebThing.Exceptions
{
    /// <summary>
    /// Base <see cref="Exception"/>.
    /// </summary>
    public class ThingException : Exception
    {
        /// <summary>
        /// Initialize a new instance of <see cref="ThingException"/>.
        /// </summary>
        public ThingException()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="ThingException"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ThingException(string? message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="ThingException"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner <see cref="Exception"/>.</param>
        public ThingException(string? message, Exception? innerException) 
            : base(message, innerException)
        {
        }
    }
}
