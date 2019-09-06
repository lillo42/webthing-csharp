using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class SetThingProperty : IWebSocketAction
    {
        private readonly IJsonSerializer _serializer;
        private readonly IJsonSerializerSettings _settings;

        public SetThingProperty(IJsonSerializer serializer, IJsonSerializerSettings settings)
        {
            _serializer = serializer;
            _settings = settings;
        }

        public string Action => "setProperty";

        public async ValueTask ExecuteAsync(Thing thing, WebSocket webSocket, IDictionary<string, object> data, CancellationToken cancellation)
        {
            foreach ((string key, object token) in data)
            {
                try
                {
                    Property property = thing.Properties.FirstOrDefault(x => x.Name == key);
                    if (property != null)
                    {
                        property.Value = token;
                    }
                }
                catch (Exception exception)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(_serializer.Serialize(new Dictionary<string, object>
                    {
                        ["messageType"] = MessageType.Error.ToString().ToLower(), 
                        ["data"] =  new Dictionary<string, object>
                        {
                            ["status"] = "400 Bad Request",
                            ["message"] = exception.Message
                        }
                    }, _settings)), WebSocketMessageType.Text, true, cancellation);
                }
            }
        }
    }
}
