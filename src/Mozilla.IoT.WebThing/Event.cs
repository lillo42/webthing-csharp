using System;

namespace Mozilla.IoT.WebThing
{
    public class Event
    {
        public Event(object data, DateTime timestamp)
        {
            Data = data;
            Timestamp = timestamp;
        }

        public object Data { get;  }

        public DateTime Timestamp { get;}
    }
}
