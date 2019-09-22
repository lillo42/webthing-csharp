using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Activator;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal sealed class DeleteActionById
    {
        internal static Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<DeleteActionById>>();
            var route = services.GetRequiredService<IHttpRouteValue>();
            logger.LogInformation("Delete is calling action");

            string thingId = route.GetValue<string>("thing");
            string name = route.GetValue<string>("name");
            string id = route.GetValue<string>("id");
            
            logger.LogInformation("Delete action: [" +
                                  $"[thing: {thingId}]" +
                                  $"[actionId: {id}]" +
                                  $"[actionName: {name}]]");

            var thing = services.GetRequiredService<IThingActivator>()
                .CreateInstance(services, thingId);

            if (thing == null)
            {
                logger.LogInformation($"Thing not found. [thing: {thingId}]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
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
