using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Middleware
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
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            await httpContext.WriteBodyAsync(HttpStatusCode.OK,
                    thing.GetEventDescriptions(httpContext.GetValueFromRoute<string>("eventName")))
                .ConfigureAwait(false);
        }
    }
}
