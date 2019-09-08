using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Activator;
using Mozilla.IoT.WebThing.Descriptor;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal sealed class GetEvents
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<GetEvents>>();
            
            logger.LogInformation("Get Events is calling");
            var route = services.GetRequiredService<IHttpRouteValue>();
            var thingId = route.GetValue<string>("thing");

            logger.LogInformation($"Get Events: [[thing: {thingId}]]");
            var thing = services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);
            
            if (thing == null)
            {
                logger.LogInformation($"Get Events: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var descriptor = services.GetService<IDescriptor<Event>>();
            
            var result = new LinkedList<Dictionary<string, object>>();

            foreach (var @event in thing.Events)
            {
                result.AddLast(new Dictionary<string, object> {[@event.Name] = descriptor.CreateDescription(@event)});
            }

            var writer = services.GetRequiredService<IHttpBodyWriter>();
            
            await writer.WriteAsync(result, httpContext.RequestAborted);

            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        }
    }
}
