using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    public abstract class Event<T> : Event
    {
        public new virtual T Data => (T) base.Data;

        protected Event(Thing thing, string name, T data) 
            : base(thing, name, data)
        {
        }

        protected Event(string name, T data) 
            : base(name, data)
        {
        }
    }

    public abstract class Event : IEquatable<Event>
    {
        /// <summary>
        /// The Event Id
        /// </summary>
        public virtual string Id { get; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// The thing associated with this event.
        /// </summary>
        public virtual Thing Thing { get; set; }
        
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

        protected internal Event()
            : this(null, null, null)
        {
            
        }
        
        protected Event(Thing thing, string name, object data)
        {
            Thing = thing;
            Name = name;
            Data = data;
        }

        protected Event(string name, object data)
        {
            Name = name;
            Data = data;
            Time = DateTime.UtcNow;
        }

        public bool Equals(Event other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Thing, other.Thing) 
                   && string.Equals(Name, other.Name) 
                   && Equals(Data, other.Data) 
                   && Time.Equals(other.Time) 
                   && Equals(Metadata, other.Metadata);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Event @event)
            {
                return Equals(@event);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Thing?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Data?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Time.GetHashCode();
                hashCode = (hashCode * 397) ^ (Metadata?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
