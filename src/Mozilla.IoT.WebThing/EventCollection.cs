using System;
using System.Collections.Concurrent;

namespace Mozilla.IoT.WebThing
{
    public class EventCollection
    {
        private readonly ConcurrentQueue<Event> _events;
        private readonly object _locker = new object();
        private readonly int _size;

        public event EventHandler<Event>? Added; 
        
        public EventCollection(int size)
        {
            _size = size;
            _events = new ConcurrentQueue<Event>();
        }

        public void Enqueue(Event @event, string name)
        {
            if (_events.Count >= _size)
            {
                lock (_locker)
                {
                    while (_events.Count >= _size)
                    {
                        _events.TryDequeue(out _);
                    }
                }
            }
    
            _events.Enqueue(@event);
            
            var add = Added;
            add?.Invoke(name, @event);
        }
        
        public void Dequeue()
        {
            _events.TryDequeue(out _);
        }

        public Event[] ToArray()
        {
            return _events.ToArray();
        }
    }
}
