using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebThing.AspNetCore.Extensions.Middlewares
{
    public class GetPropertiesThingMiddleware : AbstractThingMiddleware
    {
        public GetPropertiesThingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetPropertiesThingMiddleware>(), thingType)
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

            await OkAsync(httpContext, thing.GetPropertyDescriptions());
        }
    }
}
