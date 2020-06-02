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
    internal class GetAction
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetAction>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            var thingName = context.GetRouteData<string>("name");
            var actionName = context.GetRouteData<string>("action");
            
            logger.LogInformation("Requesting get Action. [Thing: {name}][Action: {actionName}]", 
                thingName, actionName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}][Action: {actionName}]", thingName, actionName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actionContext))
            {
                logger.LogInformation("Action not found. [Thing: {name}][Action: {actionName}]", thingName, actionName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            logger.LogInformation("Found action found. [Thing: {name}][Action: {actionName}]", thingName, actionName);

            var result = actionContext
                .Select(action => new Dictionary<string, object>
                {
                    [actionName] = action
                })
                .ToList();

            await context.WriteBodyAsync(HttpStatusCode.OK, result)
                .ConfigureAwait(false);
        }
    }
}
