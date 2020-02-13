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
            IProperties properties, 
            Dictionary<string, EventCollection> events,
            Dictionary<string, ActionContext> actions)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public IThingConverter Converter { get; }
        
        public IProperties Properties { get; }
        public LinkedList<string> PropertiesName { get; }
        public Dictionary<string, EventCollection> Events { get; }
        public Dictionary<string, ActionContext> Actions { get; } 
        public ConcurrentDictionary<Guid, WebSocket> Sockets { get; } = new ConcurrentDictionary<Guid, WebSocket>();
    }
}
