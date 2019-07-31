using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetPropertiesMiddleware : AbstractThingMiddleware
    {
        public GetPropertiesMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things) 
            : base(next, loggerFactory.CreateLogger<GetPropertiesMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Logger.LogInformation("Get Properties is calling");
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            
            Logger.LogInformation($"Get Properties: [[thing: {thingId}]]");
            var thing = Things[thingId];

            if (thing == null)
            {
                Logger.LogInformation($"Get Properties: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var result = thing.Properties.ToDictionary(property => property.Name, 
                property => property.Value);

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, result);
        }
    }
}
