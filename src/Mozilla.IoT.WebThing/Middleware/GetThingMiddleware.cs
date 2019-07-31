using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.WebSockets;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetThingMiddleware : AbstractThingMiddleware
    {
        public GetThingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things)
            : base(next, loggerFactory.CreateLogger<GetThingMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            Logger.LogInformation($"Post Action is calling: [[thing: {thingId}]");
            
            var thing = Things[thingId];

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

            var descriptor = httpContext.RequestServices.GetService<IDescription<Thing>>();
            
            IDictionary<string, object> description = descriptor.CreateDescription(thing);
            
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
