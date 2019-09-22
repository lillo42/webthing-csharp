using System.Collections.Generic;
using System.Net.WebSockets;

namespace Mozilla.IoT.WebThing
{
    internal sealed class AvailableEvent
    {
        public string Name { get; }
        public IDictionary<string, object> Metadata { get; }
        public ISet<WebSocket> Subscribers { get; }
        
        public AvailableEvent(string name, IDictionary<string, object> metadata)
        {
            Name = name;
            Metadata = metadata ?? new Dictionary<string, object>();
            Subscribers = new HashSet<WebSocket>();
        }
    }
}
