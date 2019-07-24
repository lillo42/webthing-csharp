using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Json;

using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class GetThingProperty : IWebSocketAction
    {
        private static readonly ArraySegment<byte> s_error = new ArraySegment<byte>(
            Encoding.UTF8.GetBytes(
                @"{""messageType"": ""notFound"", ""data"": {""status"": ""404 Not Found"",""message"": ""Invalid property""}}"));
        private readonly IJsonConvert _jsonConvert;
        private readonly IJsonSerializerSettings _jsonSettings;

        public GetThingProperty(IJsonSerializerSettings jsonSettings, IJsonConvert jsonConvert)
        {
            _jsonSettings = jsonSettings;
            _jsonConvert = jsonConvert;
        }
        
        public string Action => "propertyStatus";

        public async ValueTask ExecuteAsync(Thing thing, WebSocket webSocket, IDictionary<string, object> data, CancellationToken cancellation)
        {
            foreach ((string key, object token) in data)
            {
                Property property = thing.Properties.FirstOrDefault(x => x.Name == key);
                var result = property switch
                { 
                    null => new Dictionary<string, object>
                    {
                        [MESSAGE_TYPE] = MessageType.Error,
                        [DATA] = new Dictionary<string, object>
                        {
                            [STATUS] = "404 Not Found",
                            [MESSAGE] = $"Property not found: {key}"
                        }
                    },
                    _ => new Dictionary<string, object>
                    {
                        [key] = property.Value
                    }
                };

                await webSocket.SendAsync(
                    new ArraySegment<byte>(_jsonConvert.Serialize(result)),
                    WebSocketMessageType.Text, true, cancellation);
            }
        }
    }
}
