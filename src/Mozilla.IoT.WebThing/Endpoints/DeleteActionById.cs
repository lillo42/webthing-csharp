using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal static class DeleteActionById
    {
        internal static  Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetService<ILogger>();
            logger.LogInformation("Delete is calling action");
            
            var thingId =httpContext.GetValueFromRoute<string>("thing"); 
            var name = httpContext.GetValueFromRoute<string>("actionName");
            var id = httpContext.GetValueFromRoute<string>("actionId");
            
            
            logger.LogInformation("Delete action: [" +
                                  $"[thing: {thingId}]" +
                                  $"[actionId: {id}]" +
                                  $"[actionName: {name}]]");

            var thing = services.GetService<IThingActivator>().CreateInstance(services, thingId);

            if (thing == null)
            {
                logger.LogInformation($"Thing not found. [thing: {thingId}]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }

            if (thing.Actions.Contains(name) && thing.Actions[name].Remove(x => x.Id == id))
            {
                logger.LogInformation($"Action deleted. [[actionId: {id}][actionName: {name}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }
            else
            {
                logger.LogInformation($"Action not found. [[actionId: {id}][actionName: {name}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            return Task.CompletedTask;
        }
    }
}
