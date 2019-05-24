using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace WebThing.AspNetCore.Extensions.Middlewares
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
                await NotFoundAsync(httpContext);
                return;
            }
            
            string propertyName = GetPropertyName(httpContext);
            
            if (!thing.ContainsProperty(propertyName))
            {
                await NotFoundAsync(httpContext);
                return;
            }
            
            var value = thing.GetProperty(propertyName);

            await OkAsync(httpContext,
                new JObject {{propertyName, value == null ? JValue.CreateNull() : new JValue(value)}});
        }

        private static string GetPropertyName(HttpContext httpContext)
        {
            RouteData routeData = httpContext.GetRouteData();
            return routeData.Values["propertyName"].ToString();
        }
    }
}
