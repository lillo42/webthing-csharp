using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Middleware;

namespace Mozilla.IoT.WebThing.AspNetCore.Extensions.Middlewares
{
    public class DeleteActionByIdMiddleware : AbstractThingMiddleware
    {
        public DeleteActionByIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things)
            : base(next, loggerFactory.CreateLogger<DeleteActionByIdMiddleware>(), things)
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

            httpContext.Response.StatusCode =
                thing.Actions.Contains(name) && thing.Actions[name].Remove(x => x.Id == id) ?
                (int)HttpStatusCode.NoContent : (int) HttpStatusCode.NotFound;
            
            return Task.CompletedTask;
        }
    }
}
