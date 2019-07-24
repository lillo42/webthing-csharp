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
        private readonly IJsonConvert _convert;

        public SetThingProperty(IJsonConvert convert)
        {
            _convert = convert;
        }

        public string Action => "setProperty";

        public async ValueTask ExecuteAsync(Thing thing, WebSocket webSocket, IDictionary<string, object> data, CancellationToken cancellation)
        {
            foreach ((string key, object token) in data)
            {
                try
                {
                    Property property = thing.Properties.FirstOrDefault(x => x.Name == key);
                    thing.SetProperty(property, token);
                }
                catch (Exception exception)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(_convert.Serialize(new Dictionary<string, object>
                    {
                        ["messageType"] = MessageType.Error.ToString().ToLower(), 
                        ["data"] =  new Dictionary<string, object>
                        {
                            ["status"] = "400 Bad Request",
                            ["message"] = exception.Message
                        }
                    })), WebSocketMessageType.Text, true, cancellation);
                }
            }
        }
    }
}
