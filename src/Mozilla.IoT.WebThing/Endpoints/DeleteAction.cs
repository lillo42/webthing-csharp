using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class DeleteAction
    {
        public static Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<DeleteAction>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var thingName = context.GetRouteData<string>("name");
            var actionName = context.GetRouteData<string>("action");
            var id = Guid.Parse(context.GetRouteData<string>("id"));
            
            logger.LogInformation("Requesting delete Action. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                thingName, actionName, id);
            
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                    thingName, actionName, id);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }

            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actionContext))
            {
                logger.LogInformation("Action not found. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                    thingName, actionName, id);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }

            if (!actionContext.TryRemove(id, out var actionInfo))
            {
                logger.LogInformation("Action id not found. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                    thingName, actionName, id);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }

            logger.LogInformation("Going to cancel Action. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                thingName, actionName, id); 
            actionInfo.Cancel();
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            return Task.CompletedTask;
        }
    }
}
