using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Mozilla.IoT.WebThing.Events
{
    /// <summary>
    /// Queue of <see cref="Event"/>
    /// </summary>
    public class EventCollection
    {
        private readonly ConcurrentQueue<Event> _events;
        private readonly object _locker = new object();
        private readonly int _maxSize;

        /// <summary>
        /// Get the number of element in the <see cref="EventCollection"/>.
        /// </summary>
        public int Count => _events.Count;

        /// <summary>
        /// On event is added
        /// </summary>
        public event EventHandler<EventAddedEventArgs>? Added; 
        
        /// <summary>
        /// Initialize a new instance of <see cref="EventCollection"/>.
        /// </summary>
        /// <param name="maxSize">The max size of this collection.</param>
        public EventCollection(int maxSize)
        {
            _maxSize = maxSize;
            _events = new ConcurrentQueue<Event>();
        }

        /// <summary>
        /// Enqueue event.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> to be enqueue.</param>
        /// <param name="name">The name of <see cref="Event"/>.</param>
        /// <param name="thing">The <see cref="Thing"/> who dispatch the event.</param>
        public void Enqueue(Event @event, string name, Thing thing)
        {
            if (_events.Count >= _maxSize)
            {
                lock (_locker)
                {
                    while (_events.Count >= _maxSize)
                    {
                        _events.TryDequeue(out _);
                    }
                }
            }
    
            _events.Enqueue(@event);
            
            var add = Added;
            add?.Invoke(thing, new EventAddedEventArgs(name, @event));
        }
        
        /// <summary>
        /// Attempts to remove and return the object at the beginning of the <see cref="ConcurrentQueue{T}"/>.
        /// </summary>
        /// <param name="event">
        /// When this method returns, if the operation was successful, <paramref name="event"/> contains the
        /// object removed. If no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>
        /// true if an element was removed and returned from the beginning of the <see cref="ConcurrentQueue{T}"/> successfully; otherwise, false.
        /// </returns>
        public bool TryDequeue([NotNullWhen(true)]out Event? @event) 
            => _events.TryDequeue(out @event);

        /// <summary>
        /// Create event to array.
        /// </summary>
        /// <returns>New array of event.</returns>
        public Event[] ToArray()
            => _events.ToArray();
    }
}
