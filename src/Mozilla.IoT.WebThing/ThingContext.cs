using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Events;
using Mozilla.IoT.WebThing.Properties;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// Represent property, event and action the thing have.
    /// This class is used to avoid reflection.
    /// </summary>
    public class ThingContext
    {
        /// <summary>
        /// Initialize a new instance of <see cref="ThingContext"/>.
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="events">The <see cref="Dictionary{TKey,TValue}"/> with events associated with thing.</param>
        /// <param name="actions">The <see cref="Dictionary{TKey,TValue}"/> with actions associated with thing.</param>
        /// <param name="properties">The <see cref="Dictionary{TKey,TValue}"/> with properties associated with thing.</param>
        public ThingContext(IThingConverter converter, 
            Dictionary<string, EventCollection> events,
            Dictionary<string, ActionCollection> actions, 
            Dictionary<string, IProperty> properties)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        /// <summary>
        /// The <see cref="IThingConverter"/>.
        /// </summary>
        public IThingConverter Converter { get; }
        
        /// <summary>
        /// The properties associated with thing.
        /// </summary>
        public Dictionary<string, IProperty> Properties { get; }
        
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
        public ConcurrentDictionary<Guid, WebSocket> Sockets { get; } = new ConcurrentDictionary<Guid, WebSocket>();
    }
}
