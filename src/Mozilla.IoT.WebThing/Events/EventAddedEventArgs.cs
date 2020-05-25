using System;

namespace Mozilla.IoT.WebThing.Events
{
    /// <summary>
    /// The <see cref="EventArgs"/> when event had added.
    /// </summary>
    public class EventAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of <see cref="EventAddedEventArgs"/>.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="event"> The <see cref="Mozilla.IoT.WebThing.Events.Event"/>.</param>
        public EventAddedEventArgs(string eventName, Event @event)
        {
            EventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
        }

        /// <summary>
        /// The event name
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// The <see cref="Mozilla.IoT.WebThing.Events.Event"/>
        /// </summary>
        public Event Event { get; }
    }
}
