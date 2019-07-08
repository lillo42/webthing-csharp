using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.WebSockets;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetThingMiddleware : AbstractThingMiddleware
    {
        private readonly static IDictionary<string, object> s_error = new Dictionary<string, object>
        {
            ["messageType"] = "error", ["status"] = "400 Bad Request", ["message"] = "Invalid message",
        };

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

                    await process.ExecuteAsync(thing, webSocket, httpContext.RequestAborted);

                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Close sent",
                            CancellationToken.None);
                }
                catch (Exception ex)
                {
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, ex.ToString(),
                            CancellationToken.None);
                }

                return;
            }

            if (thing == null)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            string ws = string.Empty;
            
            var link = new Dictionary<string, object>
            {
                ["rel"] = "alternate",
                ["href"] = ws
            };

            IDictionary<string, object> description = thing.AsThingDescription();
            
            if(description.TryGetValue("links", out var objLinks))
            {
                if (objLinks is ICollection<IDictionary<string, object>> links)
                {
                    links.Add(link);
                }
            }
            else
            {
                description.Add("links", new List<IDictionary<string, object>>
                {
                    link
                });
            }
            
            await httpContext.WriteBodyAsync(HttpStatusCode.OK, description);
        }
    }
}
