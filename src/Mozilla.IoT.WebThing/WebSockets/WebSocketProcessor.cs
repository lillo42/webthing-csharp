using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class WebSocketProcessor
    {
        private static readonly ArraySegment<byte> s_error = new ArraySegment<byte>(Encoding.UTF8.GetBytes(new JObject
        {
            new JProperty("messageType", "error"),
            new JObject {new JProperty("status", "400 Bad Request"), new JProperty("message", "Invalid message")}
        }.ToString(Formatting.None)));

        private readonly IServiceProvider _service;

        public WebSocketProcessor(IServiceProvider service)
        {
            _service = service ?? throw new ArgumentException(nameof(service));
        }

        public async Task ExecuteAsync(Thing thing, WebSocket webSocket, CancellationToken cancellation)
        {
            thing.AddSubscriber(webSocket);

            var executors = _service.GetService<IEnumerable<IWebSocketActionExecutor>>();

            var options = _service.GetService<WebSocketOptions>();

            var buffer = ArrayPool<byte>.Shared.Rent(options.ReceiveBufferSize);

            try
            {
                WebSocketReceiveResult result = await webSocket
                    .ReceiveAsync(new ArraySegment<byte>(buffer), cancellation)
                    .ConfigureAwait(false);

                var jsonSetting = _service.GetService<JsonSerializerSettings>();

                while (!result.CloseStatus.HasValue || !cancellation.IsCancellationRequested)
                {
                    var json = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(buffer), jsonSetting);
                    
                    if (!json.ContainsKey("messageType") || !json.ContainsKey("data"))
                    {
                        await webSocket.SendAsync(s_error, WebSocketMessageType.Text, true, cancellation)
                            .ConfigureAwait(false);
                    }

                    JToken type = json["messageType"];
                    JToken data = json["data"];

                    IWebSocketActionExecutor actionExecutor = executors.FirstOrDefault(x =>  x.Action.Equals(type.Value<string>(), StringComparison.OrdinalIgnoreCase));
                    
                    if (actionExecutor != null)
                    {
                        await actionExecutor.ExecuteAsync(thing, webSocket, data as JObject, cancellation)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        await webSocket.SendAsync(s_error, WebSocketMessageType.Text, true, cancellation)
                            .ConfigureAwait(false);
                    }
                    
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
                        .ConfigureAwait(false);
                }
            }
            finally
            {
                thing.RemoveSubscriber(webSocket);
                
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
