using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class AddEventSubscriptionExecutor : IWebSocketExecutor
    {
        public string Action => "addEventSubscription";

        public Task ExecuteAsync(Thing thing, WebSocket webSocket, JObject data, CancellationToken cancellation)
        {
            foreach ((string key, JToken token) in data)
            {
                thing.AddEventSubscriber(key, webSocket);
            }

            return Task.CompletedTask;
        }
    }
}
