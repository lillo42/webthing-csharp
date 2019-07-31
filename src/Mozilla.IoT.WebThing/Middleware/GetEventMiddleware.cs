using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetEventMiddleware : AbstractThingMiddleware
    {
        public GetEventMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things) 
            : base(next, loggerFactory.CreateLogger<GetEventMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Logger.LogInformation("Get Event is calling");
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            var eventName = httpContext.GetValueFromRoute<string>("eventName");

            Logger.LogInformation($"Get Event: [[thing: {thingId}][event: {eventName}]]");
            var thing = Things[thingId];
            
            if (thing == null)
            {
                Logger.LogInformation($"Get Event: Thing not found [[thing: {thingId}][event: {eventName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var descriptor = httpContext.RequestServices.GetService<IDescription<Event>>();
            var result = thing.Events
                .Where(x => x.Name == eventName)
                .ToDictionary<Event, string, object>(@event => @event.Name,
                    @event => descriptor.CreateDescription(@event));
            
            await httpContext.WriteBodyAsync(HttpStatusCode.OK, result);
        }
    }
}
