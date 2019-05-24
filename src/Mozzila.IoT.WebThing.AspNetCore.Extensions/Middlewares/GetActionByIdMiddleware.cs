using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace WebThing.AspNetCore.Extensions.Middlewares
{
    public class GetActionByIdMiddleware : AbstractThingMiddleware
    {
        public GetActionByIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetActionByIdMiddleware>(), thingType)
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

            string name = GetActionName(httpContext);
            string id = GetActionId(httpContext);

            Action action = thing.GetAction(name, id);

            if (action == null)
            {
                await NotFoundAsync(httpContext);
                return;
            }

            await OkAsync(httpContext, action.AsActionDescription());
        }

        private static string GetActionName(HttpContext httpContext)
        {
            if (httpContext.GetRouteData().Values.TryGetValue("actionName", out object data))
            {
                return data.ToString();
            }
            
            return null;
        }

        private static string GetActionId(HttpContext httpContext) 
        {
            if (httpContext.GetRouteData().Values.TryGetValue("actionId", out object data))
            {
                return data.ToString();
            }
            
            return null;
        }
    }
}
