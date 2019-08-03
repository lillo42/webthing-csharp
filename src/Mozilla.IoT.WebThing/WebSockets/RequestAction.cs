using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Mozilla.IoT.WebThing.Activator;

namespace Mozilla.IoT.WebThing.WebSockets
{
    internal sealed class RequestAction : IWebSocketAction
    {
        private static readonly  ArraySegment<byte> s_errorMessage = new ArraySegment<byte>(Encoding.UTF8.GetBytes(@"{""messageType"": ""error"",""data"": {""status"": ""400 Bad Request"",""message"": ""Invalid action request""}}"));
        public string Action => "requestAction";
        
        private readonly ITargetBlock<Action> _target;
        private readonly IActionActivator _activator;
        private readonly IServiceProvider _provider;

        public RequestAction(ITargetBlock<Action> target, IActionActivator activator, IServiceProvider provider)
        {
            _target = target;
            _activator = activator;
            _provider = provider;
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

                Action action = _activator.CreateInstance(_provider, thing, key, input as IDictionary<string, object>);
                if (action != null)
                {
                    thing.Actions.Add(action);
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
