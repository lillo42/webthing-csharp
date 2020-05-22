using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Events;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// Represent property, event and action the thing have.
    /// This class is used to avoid reflection.
    /// </summary>
    public readonly struct ThingContext
    {
        /// <summary>
        /// Initialize a new instance of <see cref="ThingContext"/>.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="events">The <see cref="Dictionary{TKey,TValue}"/> with events associated with thing.</param>
        /// <param name="actions">The <see cref="Dictionary{TKey,TValue}"/> with actions associated with thing.</param>
        /// <param name="properties">The <see cref="Dictionary{TKey,TValue}"/> with properties associated with thing.</param>
        public ThingContext(Dictionary<string, object?> response, 
            Dictionary<string, EventCollection> events,
            Dictionary<string, ActionCollection> actions, 
            Dictionary<string, IThingProperty> properties)
        {
            Response = response ?? throw new ArgumentNullException(nameof(response));
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            Sockets = new ConcurrentDictionary<Guid, WebSocket>();
            EventsSubscribes = new Dictionary<string, ConcurrentDictionary<Guid, WebSocket>>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// The Response.
        /// </summary>
        public Dictionary<string, object?> Response { get; }
        
        /// <summary>
        /// The properties associated with thing.
        /// </summary>
        public Dictionary<string, IThingProperty> Properties { get; }
        
        /// <summary>
        /// The events associated with thing.
        /// </summary>
        public Dictionary<string, EventCollection> Events { get; }
        
        /// <summary>
        /// The actions associated with thing.
        /// </summary>
        public Dictionary<string, ActionCollection> Actions { get; } 
        
        /// <summary>
        /// The web sockets associated with thing.
        /// </summary>
        public ConcurrentDictionary<Guid, WebSocket> Sockets { get; }
        
        /// <summary>
        /// The subscribe for the events.
        /// </summary>
        public Dictionary<string, ConcurrentDictionary<Guid, WebSocket>> EventsSubscribes { get; }
    }
}
