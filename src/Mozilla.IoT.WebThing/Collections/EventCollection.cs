using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mozilla.IoT.WebThing.DebugView;

namespace Mozilla.IoT.WebThing.Collections
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class EventCollection : IEventCollection, IEquatable<EventCollection>
    {
        private readonly LinkedList<Event> _events = new LinkedList<Event>();
        private readonly object _locker = new object();

        public IEnumerator<Event> GetEnumerator()
            => _events.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Add(Event item)
        {
            lock (_locker)
            {
                _events.AddLast(item);
            }

            var @event = EventAdded;
            @event?.Invoke(this, new EventAddedEventArgs(item));
        }

        public void Clear()
        {
            lock (_locker)
            {
                _events.Clear();
            }
        }

        public bool Contains(Event item)
            => _events.Contains(item);

        public void CopyTo(Event[] array, int arrayIndex)
            => _events.CopyTo(array, arrayIndex);

        public bool Remove(Event item)
        {
            bool result = false;

            lock (_locker)
            {
                result = _events.Remove(item);
            }
            
            return result;
        }

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
