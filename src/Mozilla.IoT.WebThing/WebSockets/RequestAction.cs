using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class RequestAction : IWebSocketAction
    {
        private static readonly  ArraySegment<byte> s_errorMessage = new ArraySegment<byte>(Encoding.UTF8.GetBytes(@"{""messageType"": ""error"",""data"": {""status"": ""400 Bad Request"",""message"": ""Invalid action request""}}"));
        public string Action => "requestAction";

        private readonly IJsonConvert _convert;
        private readonly ITargetBlock<Action> _target;
        private readonly IServiceProvider _service;

        public RequestAction(ITargetBlock<Action> target, IJsonConvert convert, IServiceProvider service)
        {
            _target = target;
            _convert = convert;
            _service = service;
        }

        public async ValueTask ExecuteAsync(Thing thing, WebSocket webSocket, IDictionary<string, object> data, CancellationToken cancellation)
        {
            foreach ((string key, object token) in data)
            {
                object input = null;
                if (token is IDictionary<string, object> body && body.ContainsKey("input"))
                {
                    input = body["input"];
                }

                Action action = thing.GetAction(key, input as IDictionary<string, object>, _service);
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
