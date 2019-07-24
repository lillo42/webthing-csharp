using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class RequestAction : IWebSocketAction
    {
        private static readonly  ArraySegment<byte> s_errorMessage = new ArraySegment<byte>(Encoding.UTF8.GetBytes(@"{""messageType"": ""error"",""data"": {""status"": ""400 Bad Request"",""message"": ""Invalid action request""}}"));
        public string Action => "requestAction";
        
        private readonly ITargetBlock<Action> _target;
        private readonly IActionFactory _factory;

        public RequestAction(ITargetBlock<Action> target, IActionFactory factory)
        {
            _target = target;
            _factory = factory;
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

                Action action = await _factory.CreateAsync(thing, key, input as IDictionary<string, object>, cancellation);
                if (action != null)
                {
                    await _target.SendAsync(action, cancellation);
                }
                else
                {
                    await webSocket.SendAsync(s_errorMessage, WebSocketMessageType.Text, true, cancellation);
                }
            }
        }
    }
}
