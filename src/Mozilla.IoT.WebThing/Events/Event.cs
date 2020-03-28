using System;

namespace Mozilla.IoT.WebThing.Events
{
    /// <summary>
    /// Represent raised event.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Initialize a new instance of <see cref="Event"/>.
        /// </summary>
        /// <param name="data">The value raised.</param>
        public Event(object data)
        {
            Data = data;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Event value.
        /// </summary>
        public object Data { get;  }

        /// <summary>
        /// Datetime the event was raised.
        /// </summary>
        public DateTime Timestamp { get; }
    }
}
