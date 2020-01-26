using System.Collections.Concurrent;

namespace Mozilla.IoT.WebThing
{
    public class ThingEventCollection
    {
        private readonly ConcurrentBag<ThingEvent> _events;
        private readonly int _size;
        private readonly object _locker = new object();

        public ThingEventCollection(int size)
        {
            _size = size;
            _events = new ConcurrentBag<ThingEvent>();
        }

        public void Add(ThingEvent thingEvent)
        {
            if (_events.Count == _size)
            {
                _events.TryTake(out _);
            }
    
            _events.Add(thingEvent);
        }

        public ThingEvent[] ToArray()
        {
            return _events.ToArray();
        }
    }
}
