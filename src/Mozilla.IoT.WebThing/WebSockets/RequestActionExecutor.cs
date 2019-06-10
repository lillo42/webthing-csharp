using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class RequestActionExecutor : IWebSocketActionExecutor
    {
        private static readonly  ArraySegment<byte> s_errorMessage = new ArraySegment<byte>(Encoding.UTF8.GetBytes(@"{""messageType"": ""error"",""data"": {""status"": ""400 Bad Request"",""message"": ""Invalid action request""}}"));
        public string Action => "requestAction";

        private readonly ITargetBlock<Action> _target;

        public RequestActionExecutor(ITargetBlock<Action> target)
        {
            _target = target;
        }

        public async Task ExecuteAsync(Thing thing, WebSocket webSocket, JObject data, CancellationToken cancellation)
        {
            foreach ((string key, JToken token) in data)
            {
                JObject body = token.Value<JObject>();

                JObject input = null;
                if (body.ContainsKey("input"))
                {
                    input = body["input"] as JObject;
                }

                Action action = await thing.PerformActionAsync(key, input, cancellation)
                    .ConfigureAwait(false);

                if (action != null)
                {
                    await _target.SendAsync(action, cancellation)
                        .ConfigureAwait(false);
                }
                else
                {
                    await webSocket.SendAsync(s_errorMessage, WebSocketMessageType.Text, true, cancellation)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
