using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class AddEventSubscriptionActionExecutor : IWebSocketActionExecutor
    {
        public string Action => "addEventSubscription";

        public Task ExecuteAsync(Thing thing, WebSocket webSocket, IDictionary<string, object> data, CancellationToken cancellation)
        {
            foreach (var keyPair in data)
            {
                thing.AddEventSubscriber(keyPair.Key
                    , webSocket);
            }

            return Task.CompletedTask;
        }
    }
}
