using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace WebThing.AspNetCore.Extensions.Middlewares
{
    public class GetActionsMiddleware : AbstractThingMiddleware
    {
        public GetActionsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetActionsMiddleware>(), thingType)
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
            
            await OkAsync(httpContext, thing.GetActionDescriptions().ToString());
        }

        private static string GetActionName(HttpContext httpContext)
        {
            if (httpContext.GetRouteData().Values.TryGetValue("actionName", out object data))
            {
                return data.ToString();
            }
            
            return null;
        }
    }
}
