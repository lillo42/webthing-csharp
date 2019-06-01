using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.AspNetCore.Extensions.Middlewares
{
    public class GetThingMiddleware : AbstractThingMiddleware
    {
        public GetThingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType)
            : base(next, loggerFactory.CreateLogger<GetThingMiddleware>(), thingType)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                thing.AddSubscriber(webSocket);

                var actions = httpContext.RequestServices.GetService<IDictionary<string, IWebSocketExecutor>>();

                var buffer = new byte[1024 * 4];

                WebSocketReceiveResult result = await webSocket
                    .ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
                    .ConfigureAwait(false);

                while (!result.CloseStatus.HasValue)
                {
                    await ReadWebSocketAsync(thing, webSocket, buffer, actions)
                        .ConfigureAwait(false);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
                        .ConfigureAwait(false);
                }
            }

            if (thing == null)
            {
                await NotFoundAsync(httpContext);
                return;
            }

            string ws = string.Empty;
            var link = new JObject(
                new JProperty("rel", "alternate"),
                new JProperty("href", ws));

            JObject description = thing.AsThingDescription();

            (description["links"] as JArray)?.Add(link);

            await OkAsync(httpContext, description);
        }

        private static async Task ReadWebSocketAsync(Thing thing, WebSocket webSocket, byte[] buffer,
            IDictionary<string, IWebSocketExecutor> actions)
        {
            JObject json = JsonConvert.DeserializeObject<JObject>(Encoding.Default.GetString(buffer));

            if (!json.ContainsKey("messageType") || !json.ContainsKey("data"))
            {
                var error = new JObject
                {
                    new JProperty("messageType", "error"),
                    new JObject
                    {
                        new JProperty("status", "400 Bad Request"), new JProperty("message", "Invalid message")
                    }
                };

                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.Default.GetBytes(error.ToString())),
                        WebSocketMessageType.Text, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }

            JToken type = json["messageType"];
            JToken data = json["data"];

            if (actions.TryGetValue(type.Value<string>(), out var executor))
            {
                await executor.ExecuteAsync(thing, webSocket, data as JObject, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else
            {
                await webSocket.SendAsync(
                        new ArraySegment<byte>(Encoding.Default.GetBytes(new JObject
                        {
                            new JProperty("messageType", "error"),
                            new JObject
                            {
                                new JProperty("status", "400 Bad Request"),
                                new JProperty("message", "Invalid message")
                            }
                        }.ToString())),
                        WebSocketMessageType.Text, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }
    }
}
