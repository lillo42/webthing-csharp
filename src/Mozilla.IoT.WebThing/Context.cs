using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing
{
    public class Context
    {
        public Context(IThingConverter converter, 
            Dictionary<string, EventCollection> events,
            Dictionary<string, ActionCollection> actions, 
            Dictionary<string, IProperty> properties)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        public IThingConverter Converter { get; }
        public Dictionary<string, IProperty> Properties { get; }
        public Dictionary<string, EventCollection> Events { get; }
        public Dictionary<string, ActionCollection> Actions { get; } 
        public ConcurrentDictionary<Guid, WebSocket> Sockets { get; } = new ConcurrentDictionary<Guid, WebSocket>();
    }
}
