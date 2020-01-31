using System.Collections.Concurrent;

namespace Mozilla.IoT.WebThing
{
    public class ThingEventCollection
    {
        private readonly ConcurrentQueue<ThingEvent> _events;
        private readonly int _size;

        public ThingEventCollection(int size)
        {
            _size = size;
            _events = new ConcurrentQueue<ThingEvent>();
        }

        public void Add(ThingEvent thingEvent)
        {
            if (_events.Count >= _size)
            {
                _events.TryDequeue(out _);
            }
    
            _events.Enqueue(thingEvent);
        }

        public ThingEvent[] ToArray()
        {
            return _events.ToArray();
        }
    }
}
