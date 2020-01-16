using System;

namespace Mozilla.IoT.WebThing
{
    public abstract class Event : IEquatable<Event>
    {
        #region Properties
        /// <summary>
        /// The name of event
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// The event data.
        /// </summary>
        public virtual object? Data { get; }

        /// <summary>
        /// The event timestamp
        /// </summary>
        public virtual DateTime Timestamp { get; }
        #endregion

        #region Constructor

        protected Event()
        {
            
        }
        
        protected Event(string name, object? data)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Data = data;
            Timestamp = DateTime.UtcNow;
        }

        protected Event(string name, object? data, DateTime timestamp)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Data = data;
            Timestamp = timestamp;
        }

        #endregion

        #region IEquatable
        public bool Equals(Event? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name 
                   && Equals(Data, other.Data) 
                   && Timestamp.Equals(other.Timestamp);
        }

        public override bool Equals(object? obj)
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
            => HashCode.Combine(Name, Data, Timestamp);

        #endregion
        
        #region Operator
        public static bool operator ==(Event obj1, Event obj2) =>
            obj1 switch
            {
                null when obj2 is null => true,
                null => false,
                _ => obj1.Equals(obj2)
            };

        public static bool operator !=(Event obj1, Event obj2) 
            => !(obj1 == obj2);

        #endregion
    }

    public abstract class Event<T> : Event
    {
        /// <summary>
        /// The event data.
        /// </summary>
        public new virtual T Data
        {
            get
            {
                var result = base.Data;
                if (result == null)
                {
                    return default;
                }

                return (T)result;
            }
        }
        
        #region Constructor

        protected Event(string name, T data)
            : base(name, data)
        {
            
        }

        protected Event(string name, T data, DateTime timestamp)
            : base(name, data, timestamp)
        {
          
        }

        #endregion
        
        #region Operator
        public static bool operator ==(Event<T> obj1, Event<T> obj2) =>
            obj1 switch
            {
                null when obj2 is null => true,
                null => false,
                _ => obj1.Equals(obj2)
            };

        public static bool operator !=(Event<T> obj1, Event<T> obj2) 
            => !(obj1 == obj2);

        #endregion
    }
}
