using System.Collections.Concurrent;

namespace Mozilla.IoT.WebThing
{
    public class EventCollection
    {
        private readonly ConcurrentBag<Event> _events;
        private readonly int _size;
        private readonly object _locker = new object();

        public EventCollection(int size)
        {
            _size = size;
            _events = new ConcurrentBag<Event>();
        }

        public void Add(Event @event)
        {
            if (_events.Count == _size)
            {
                _events.TryTake(out _);
            }
    
            _events.Add(@event);
        }

        public Event[] ToArray()
        {
            return _events.ToArray();
        }
    }
}
