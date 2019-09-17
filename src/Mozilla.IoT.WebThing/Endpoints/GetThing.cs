using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Activator;
using Mozilla.IoT.WebThing.Builder;
using Mozilla.IoT.WebThing.Descriptor;
using Mozilla.IoT.WebThing.WebSockets;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal sealed class GetThing
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<GetThing>>();

            var route = services.GetRequiredService<IHttpRouteValue>();
            string thingId = route.GetValue<string>("thing");
            logger.LogInformation($"Post Action is calling: [[thing: {thingId}]");

            var thing = services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);

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
            
            var builder = services.GetService<IWsUrlBuilder>();

            var link = new Dictionary<string, object>
            {
                ["rel"] = "alternate", 
                ["href"] = builder.Build(httpContext.Request, thingId)
            };

            var descriptor = services.GetService<IDescriptor<Thing>>();

            var description = descriptor.CreateDescription(thing);

            if (description.TryGetValue("links", out var objLinks))
            {
                if (objLinks is ICollection<IDictionary<string, object>> links)
                {
                    links.Add(link);
                }
            }
            else
            {
                description.Add("links", new List<IDictionary<string, object>> {link});
            }

            var writer = services.GetRequiredService<IHttpBodyWriter>();
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            await writer.WriteAsync(description, httpContext.RequestAborted);
        }
    }
}
