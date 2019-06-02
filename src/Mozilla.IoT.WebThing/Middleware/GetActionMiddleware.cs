using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetActionMiddleware : AbstractThingMiddleware
    {
        public GetActionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetActionMiddleware>(), thingType)
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
                    thing.GetActionDescriptions(httpContext.GetValueFromRoute<string>("actionName")))
                .ConfigureAwait(false);
        }
    }
}
