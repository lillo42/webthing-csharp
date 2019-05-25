using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Mozzila.IoT.WebThing
{
    /// <summary>
    /// An Event represents an individual event from a thing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Event<T> : Event
    {
        public new T Data => (T)base.Data;

        public Event(Thing thing, string name)
            : base(thing, name)
        {
        }

        public Event(Thing thing, string name, T data)
            : base(thing, name, data)
        {
        }
    }

    /// <summary>
    /// An Event represents an individual event from a thing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Event
    {
        private const string TIMESTAMP = "timestamp";
        private const string DATA = "data";


        /// <summary>
        /// The thing associated with this event.
        /// </summary>
        public Thing Thing { get; }

        /// <summary>
        /// The event's name. 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The event's data.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// The event's timestamp.
        /// </summary>
        public DateTime Time { get; }

        public Event(Thing thing, string name)
            : this(thing, name, null)
        {
        }

        public Event(Thing thing, string name, object data)
        {
            Thing = thing;
            Name = name;
            Data = data;
            Time = DateTime.UtcNow;
        }

        /// <summary>
        /// Get the event description. 
        /// </summary>
        /// <returns>Description of the event as a JObject.</returns>
        public JObject AsEventDescription()
        {
            var inner = new JObject(new JProperty(TIMESTAMP, Time));

            if (Data != null)
            {
                inner.Add(new JProperty(DATA, Data));
            }

            return new JObject(new JProperty(Name, inner));
        }
    }
}
