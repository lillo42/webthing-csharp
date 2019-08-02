using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal static class GetEvents
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetService<ILogger>();
            
            logger.LogInformation("Get Events is calling");
            var thingId = httpContext.GetValueFromRoute<string>("thing");

            logger.LogInformation($"Get Events: [[thing: {thingId}]]");
            var thing = services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);
            
            if (thing == null)
            {
                logger.LogInformation($"Get Events: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var descriptor = services.GetService<IDescription<Event>>();
            
            var result = thing.Events
                .ToDictionary<Event, string, object>(@event => @event.Name, 
                    @event => descriptor.CreateDescription(@event));

            await httpContext.WriteBodyAsync(HttpStatusCode.OK,result);
        }
    }
}
