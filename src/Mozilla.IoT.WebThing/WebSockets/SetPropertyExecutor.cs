using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class SetPropertyExecutor : IWebSocketExecutor
    {
        public string Action => "setProperty";

        public async Task ExecuteAsync(Thing thing, WebSocket webSocket, JObject data, CancellationToken cancellation)
        {
            foreach ((string key, JToken token) in data)
            {
                try
                {
                    thing.SetProperty(key, token.Value<object>());
                }
                catch (Exception e)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.Default.GetBytes(new JObject
                            {
                                {"messageType", "error"},
                                {
                                    "data", new JObject(
                                        new JProperty("status", "400 Bad Request"),
                                        new JProperty("message", e.Message))
                                }
                            }
                            .ToString())), WebSocketMessageType.Text, true, cancellation)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
