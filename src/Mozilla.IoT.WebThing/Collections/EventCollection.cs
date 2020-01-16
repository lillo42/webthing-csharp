using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mozilla.IoT.WebThing.DebugView;

namespace Mozilla.IoT.WebThing.Collections
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class EventCollection : IEventCollection, IEquatable<EventCollection>
    {
        private readonly ConcurrentDictionary<string, Event> _events = new ConcurrentDictionary<string, Event>();
        private readonly object _locker = new object();

        public IEnumerator<Event> GetEnumerator()
            => _events.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Add(Event item)
        {
            _events.TryAdd(item.Id, item);
            var @event = EventAdded;
            @event?.Invoke(this, new EventAddedEventArgs(item));
        }

        public void Clear() 
            => _events.Clear();

        public bool Contains(Event item) 
            => _events.ContainsKey(item.Id);

        public void CopyTo(Event[] array, int arrayIndex)
            => _events.Values.CopyTo(array, arrayIndex);

        public bool Remove(Event item) 
            => _events.TryRemove(item.Id, out _);

        public int Count => _events.Count;
        public bool IsReadOnly => false;

        public bool Equals(EventCollection other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(_events, other._events);
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

            return obj.GetType() == GetType() && Equals((EventCollection)obj);
        }

        public override int GetHashCode()
            => (_events != null ? _events.GetHashCode() : 0);

        public event EventHandler<EventAddedEventArgs> EventAdded;
    }
}
