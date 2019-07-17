using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class SetPropertyActionExecutor : IWebSocketActionExecutor
    {
        public string Action => "setProperty";

        public async Task ExecuteAsync(Thing thing, WebSocket webSocket, IDictionary<string, object> data, CancellationToken cancellation)
        {
            foreach ((string key, object token) in data)
            {
                try
                {
                    thing.SetProperty(key, token);
                }
                catch (Exception e)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.Default.GetBytes(new Dictionary<string, object>()
                            {
                                ["messageType"] = "error", 
                                ["data"] =  new Dictionary<string, object>
                                {
                                    ["status"] = "400 Bad Request",
                                    ["message"] = e.Message
                                }
                            }
                            .ToString())), WebSocketMessageType.Text, true, cancellation)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
