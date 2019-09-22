using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class SetThingProperty : IWebSocketAction
    {
        private readonly IJsonSerializer _serializer;

        public SetThingProperty(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public string Action => "setProperty";

        public ValueTask ExecuteAsync(Thing thing, WebSocket webSocket, IDictionary<string, object> data, CancellationToken cancellation)
        {
            if (data == null)
            {
                return new ValueTask();
            }
            
            var tasks = new LinkedList<Task>();

            foreach ((string key, object token) in data)
            {
                try
                {
                    thing.Properties.SetProperty(key, token);
                }
                catch (Exception exception)
                {
                    tasks.AddLast(webSocket.SendAsync(new ArraySegment<byte>(_serializer.Serialize(new Dictionary<string, object>
                    {
                        ["messageType"] = MessageType.Error.ToString().ToLower(), 
                        ["data"] =  new Dictionary<string, object>
                        {
                            ["status"] = "400 Bad Request",
                            ["message"] = exception.Message
                        }
                    })), WebSocketMessageType.Text, true, cancellation));
                }
            }

            return tasks.Count == 0 ? new ValueTask() : new ValueTask(Task.WhenAll(tasks));
        }
    }
}
