using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace WebThing.AspNetCore.Extensions.Middlewares
{
    public class GetActionIdMiddleware : AbstractThingMiddleware
    {
        public GetActionIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetActionIdMiddleware>(), thingType)
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
            => httpContext.GetRouteData().Values["actionName"].ToString();
        
        private static string GetActionId(HttpContext httpContext) 
            => httpContext.GetRouteData().Values["actionId"].ToString();
    }
}
