using System.Collections.Generic;
using System.Net.WebSockets;

namespace Mozilla.IoT.WebThing
{
    public partial class Thing
    {
        private class AvailableEvent
        {
            public IDictionary<string, object> Metadata { get; }
            public ISet<WebSocket> Subscribers { get; } = new HashSet<WebSocket>();

            public AvailableEvent(IDictionary<string, object> metadata)
            {
                Metadata = metadata;
            }

            public void AddSubscriber(WebSocket ws)
                => Subscribers.Add(ws);

            public void RemoveSubscriber(WebSocket ws)
            {
                if (Subscribers.Contains(ws))
                {
                    Subscribers.Remove(ws);
                }
            }
        }
    }
}
