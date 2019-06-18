using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetThingMiddleware : AbstractThingMiddleware
    {
        private static readonly ArraySegment<byte> s_error = new ArraySegment<byte>(Encoding.UTF8.GetBytes(new JObject
        {
            new JProperty("messageType", "error"),
            new JObject {new JProperty("status", "400 Bad Request"), new JProperty("message", "Invalid message")}
        }.ToString(Formatting.None)));

        public GetThingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType)
            : base(next, loggerFactory.CreateLogger<GetThingMiddleware>(), thingType)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync()
                    .ConfigureAwait(false);
                try
                {
                    var process = httpContext.RequestServices.GetService<WebSocketProcessor>();
                    
                    await process.ExecuteAsync(thing, webSocket, httpContext.RequestAborted)
                        .ConfigureAwait(false);
                    
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Close sent", CancellationToken.None)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, ex.ToString(), CancellationToken.None)
                        .ConfigureAwait(false);
                }
                return;
                
            }

            if (thing == null)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            string ws = string.Empty;
            var link = new JObject(
                new JProperty("rel", "alternate"),
                new JProperty("href", ws));

            JObject description = thing.AsThingDescription();

            ((JArray)description["links"]).Add(link);

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, description)
                .ConfigureAwait(false);
        }
    }
}
