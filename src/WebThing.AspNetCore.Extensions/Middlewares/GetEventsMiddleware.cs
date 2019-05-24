using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace WebThing.AspNetCore.Extensions.Middlewares
{
    public class GetEventMiddleware : AbstractThingMiddleware
    {
        public GetEventMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetEventMiddleware>(), thingType)
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
            
            await OkAsync(httpContext, thing.GetEventDescriptions(GetEventName(httpContext)).ToString());
        }

        private static string GetEventName(HttpContext httpContext)
        {
            if (httpContext.GetRouteData().Values.TryGetValue("eventName", out object data))
            {
                return data.ToString();
            }
            
            return null;
        }
    }
}
