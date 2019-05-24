using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using Newtonsoft.Json.Linq;

namespace Mozzila.IoT.WebThing
{
    public partial class Thing
    {
        private class AvailableEvent
        {
            public JObject Metadata { get; }
            public ISet<WebSocket> Subscribers { get; } = new HashSet<WebSocket>();

            public AvailableEvent(JObject metadata)
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
