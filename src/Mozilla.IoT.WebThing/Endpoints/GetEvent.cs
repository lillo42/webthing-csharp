using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal static class GetEvent
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetService<ILogger>();
            
            logger.LogInformation("Get Event is calling");
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            var eventName = httpContext.GetValueFromRoute<string>("name");

            logger.LogInformation($"Get Event: [[thing: {thingId}][event: {eventName}]]");
            var thing = services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);
            
            if (thing == null)
            {
                logger.LogInformation($"Get Event: Thing not found [[thing: {thingId}][event: {eventName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var descriptor = services.GetService<IDescriptor<Event>>();
            var result = thing.Events
                .Where(x => x.Name == eventName)
                .ToDictionary<Event, string, object>(@event => @event.Name,
                    @event => descriptor.CreateDescription(@event));
            
            await httpContext.WriteBodyAsync(HttpStatusCode.OK, result);
        }
    }
}
