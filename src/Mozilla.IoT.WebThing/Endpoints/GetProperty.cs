using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Activator;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal sealed class GetProperty
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<GetProperty>>();
            
            logger.LogInformation("Get Property is calling");
            var route = services.GetRequiredService<IHttpRouteValue>();
            var thingId = route.GetValue<string>("thing");
            var propertyName = route.GetValue<string>("name");

            logger.LogInformation($"Get Property: [[thing: {thingId}][property: {propertyName}]]");
            var thing =  services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);

            if (thing == null)
            {
                logger.LogInformation($"Get Property: Thing not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var property = thing.Properties.FirstOrDefault(x => x.Name == propertyName);
            
            if (property == null)
            {
                logger.LogInformation($"Get Property: Property not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            var writer = services.GetRequiredService<IHttpBodyWriter>();
            httpContext.Response.StatusCode = (int) HttpStatusCode.OK;
            await writer.WriteAsync(new Dictionary<string, object>
            {
                [propertyName] =  property.Value
            }, httpContext.RequestAborted);
        }
    }
}
