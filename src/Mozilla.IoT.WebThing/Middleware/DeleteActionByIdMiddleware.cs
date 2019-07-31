using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;

namespace Mozilla.IoT.WebThing.Middleware
{
    internal sealed class DeleteActionByIdMiddleware : AbstractThingMiddleware
    {
        public DeleteActionByIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things)
            : base(next, loggerFactory.CreateLogger<DeleteActionByIdMiddleware>(), things)
        {
        }

        public Task Invoke(HttpContext httpContext)
        {
            Logger.LogInformation("Delete is calling action");
            
            var thingId =httpContext.GetValueFromRoute<string>("thing"); 
            var name = httpContext.GetValueFromRoute<string>("actionName");
            var id = httpContext.GetValueFromRoute<string>("actionId");
            
            
            Logger.LogInformation("Delete action: [" +
                                  $"[thing: {thingId}]" +
                                  $"[actionId: {id}]" +
                                  $"[actionName: {name}]]");

            var thing = Things[thingId];

            if (thing == null)
            {
                Logger.LogInformation($"Thing not found. [thing: {thingId}]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }

            if (thing.Actions.Contains(name) && thing.Actions[name].Remove(x => x.Id == id))
            {
                Logger.LogInformation($"Action deleted. [[actionId: {id}][actionName: {name}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }
            else
            {
                Logger.LogInformation($"Action not found. [[actionId: {id}][actionName: {name}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            return Task.CompletedTask;
        }
    }
}
