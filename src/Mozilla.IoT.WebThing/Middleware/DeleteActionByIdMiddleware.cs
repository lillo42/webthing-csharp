using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Middleware;

namespace Mozilla.IoT.WebThing.AspNetCore.Extensions.Middlewares
{
    public class DeleteActionByIdMiddleware : AbstractThingMiddleware
    {
        public DeleteActionByIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType)
            : base(next, loggerFactory.CreateLogger<DeleteActionByIdMiddleware>(), thingType)
        {
        }

        public Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (thing == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }

            string name = httpContext.GetValueFromRoute<string>("actionName");
            string id = httpContext.GetValueFromRoute<string>("actionId");

            httpContext.Response.StatusCode = thing.RemoveAction(name, id) ? 
                (int)HttpStatusCode.NoContent : (int) HttpStatusCode.NotFound;
            
            return Task.CompletedTask;
        }
    }
}
