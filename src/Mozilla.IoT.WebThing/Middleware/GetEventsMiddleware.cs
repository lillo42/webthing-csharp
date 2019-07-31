using System.Collections.Generic;
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
    public class GetEventsMiddleware : AbstractThingMiddleware
    {
        public GetEventsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things) 
            : base(next, loggerFactory.CreateLogger<GetEventsMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Logger.LogInformation("Get Events is calling");
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            
            Logger.LogInformation($"Get Events: [[thing: {thingId}]]");
            var thing = Things[thingId];

            if (thing == null)
            {
                Logger.LogInformation($"Get Events: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var descriptor = httpContext.RequestServices.GetService<IDescription<Event>>();
            
            var result = thing.Events
                .ToDictionary<Event, string, object>(@event => @event.Name, 
                    @event => descriptor.CreateDescription(@event));

            await httpContext.WriteBodyAsync(HttpStatusCode.OK,result);
        }
    }
}
