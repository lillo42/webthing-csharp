using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    public class Event<T> : Event
    {
        public new virtual T Data => (T) base.Data;

        public Event(Thing thing, string name, T data) 
            : base(thing, name, data)
        {
        }

        public Event(string name, T data) 
            : base(name, data)
        {
        }
    }
    
    public class Event
    {
        /// <summary>
        /// The thing associated with this event.
        /// </summary>
        public Thing Thing { get; internal set; }
        
        /// <summary>
        /// The event's name. 
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// The event's data.
        /// </summary>
        public virtual object Data { get; }

        /// <summary>
        /// The event's timestamp.
        /// </summary>
        public virtual DateTime Time { get; }

        internal IDictionary<string, object> Metadata { get; set; }

        public Event(Thing thing, string name, object data)
        {
            Thing = thing;
            Name = name;
            Data = data;
        }

        public Event(string name, object data)
        {
            Name = name;
            Data = data;
            Time = DateTime.UtcNow;
        }
    }
}
