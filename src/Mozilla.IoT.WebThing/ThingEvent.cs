using System;

namespace Mozilla.IoT.WebThing
{
    public class ThingEvent
    {
        public ThingEvent(object data)
        {
            Data = data;
            Timestamp = DateTime.UtcNow;
        }

        public object Data { get;  }

        public DateTime Timestamp { get;}
    }
}
