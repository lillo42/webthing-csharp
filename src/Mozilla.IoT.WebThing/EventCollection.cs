using System.Collections.Concurrent;

namespace Mozilla.IoT.WebThing
{
    public class EventCollection
    {
        private readonly ConcurrentQueue<Event> _events;
        private readonly int _size;

        public EventCollection(int size)
        {
            _size = size;
            _events = new ConcurrentQueue<Event>();
        }

        public void Add(Event @event)
        {
            if (_events.Count >= _size)
            {
                _events.TryDequeue(out _);
            }
    
            _events.Enqueue(@event);
        }

        public Event[] ToArray()
        {
            return _events.ToArray();
        }
    }
}
