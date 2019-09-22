using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class AddEventSubscription : IWebSocketAction
    {
        public string Action => "addEventSubscription";
        public ValueTask ExecuteAsync(Thing thing, WebSocket webSocket, IDictionary<string, object> data, CancellationToken cancellation)
        {
            foreach (var keyPair in data)
            {
                if (thing.AvailableEvent.ContainsKey(keyPair.Key))
                {
                    thing.AvailableEvent[keyPair.Key].Subscribers.Add(webSocket);
                }
            }
            
            return new ValueTask();
        }
    }
}
