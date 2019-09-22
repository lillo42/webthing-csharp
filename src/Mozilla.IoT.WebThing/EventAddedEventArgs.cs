using System;

namespace Mozilla.IoT.WebThing
{
    public sealed class EventAddedEventArgs : EventArgs
    {
        public EventAddedEventArgs(Event @event)
        {
            Event = @event;
        }

        public Event Event { get; }
    }
}
