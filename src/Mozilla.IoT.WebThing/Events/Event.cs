using System;

namespace Mozilla.IoT.WebThing.Events
{
    public class Event
    {
        public Event(object data)
        {
            Data = data;
            Timestamp = DateTime.UtcNow;
        }

        public object Data { get;  }

        public DateTime Timestamp { get;}
    }
}
