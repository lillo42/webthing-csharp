using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.AspNetCore.Extensions.Middlewares
{
    public class GetPropertiesMiddleware : AbstractThingMiddleware
    {
        public GetPropertiesMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetPropertiesMiddleware>(), thingType)
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
