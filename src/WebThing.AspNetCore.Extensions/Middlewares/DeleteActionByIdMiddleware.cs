using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace WebThing.AspNetCore.Extensions.Middlewares
{
    public class DeleteActionIdMiddleware : AbstractThingMiddleware
    {
        public DeleteActionIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType)
            : base(next, loggerFactory.CreateLogger<DeleteActionIdMiddleware>(), thingType)
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

            if (thing.RemoveAction(name, id))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }
            else
            {
                await NotFoundAsync(httpContext);
            }
        }

        private static string GetActionName(HttpContext httpContext)
            => httpContext.GetRouteData().Values["actionName"].ToString();

        private static string GetActionId(HttpContext httpContext)
            => httpContext.GetRouteData().Values["actionId"].ToString();
    }
}
