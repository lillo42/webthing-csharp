using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.AspNetCore.Extensions.Middlewares;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetPropertyThingMiddleware : AbstractThingMiddleware
    {
        public GetPropertyThingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType)
            : base(next, loggerFactory.CreateLogger<GetPropertyThingMiddleware>(), thingType)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);
            
            if (thing == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            string propertyName = httpContext.GetValueFromRoute<string>("propertyName");
            
            if (!thing.ContainsProperty(propertyName))
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var value = thing.GetProperty(propertyName);

            await httpContext.WriteBodyAsync(HttpStatusCode.OK,
                    new JObject {{propertyName, value == null ? JValue.CreateNull() : new JValue(value)}})
                .ConfigureAwait(false);
        }
    }
}
