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
    internal sealed class GetEvent
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<GetEvent>>();

            logger.LogInformation("Get Event is calling");
            var route = services.GetRequiredService<IHttpRouteValue>();
            var thingId = route.GetValue<string>("thing");
            var eventName = route.GetValue<string>("name");

            logger.LogInformation($"Get Event: [[thing: {thingId}][event: {eventName}]]");
            var thing = services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);

            if (thing == null)
            {
                logger.LogInformation($"Get Event: Thing not found [[thing: {thingId}][event: {eventName}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var descriptor = services.GetService<IDescriptor<Event>>();
            var result = new LinkedList<Dictionary<string, object>>();

            foreach (var @event in thing.Events.Where(x => x.Name == eventName))
            {
                result.AddLast(new Dictionary<string, object> {[@event.Name] = descriptor.CreateDescription(@event)});
            }

            var writer = services.GetRequiredService<IHttpBodyWriter>();
            httpContext.Response.StatusCode = (int) HttpStatusCode.OK;
            await writer.WriteAsync(result, httpContext.RequestAborted);
        }
    }
}
