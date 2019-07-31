using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetPropertyThingMiddleware : AbstractThingMiddleware
    {
        public GetPropertyThingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things)
            : base(next, loggerFactory.CreateLogger<GetPropertyThingMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Logger.LogInformation("Get Property is calling");
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            var propertyName = httpContext.GetValueFromRoute<string>("propertyName");

            Logger.LogInformation($"Get Property: [[thing: {thingId}][property: {propertyName}]]");
            var thing = Things[thingId];

            if (thing == null)
            {
                Logger.LogInformation($"Get Property: Thing not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var property = thing.Properties.FirstOrDefault(x => x.Name == propertyName);
            
            if (property == null)
            {
                Logger.LogInformation($"Get Property: Property not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, new Dictionary<string, object>
                    {
                        [propertyName] =  property.Value
                    });
        }
    }
}
