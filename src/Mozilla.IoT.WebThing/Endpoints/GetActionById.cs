using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetActionById
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetActionById>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var thingName = context.GetRouteData<string>("name");
            var actionName = context.GetRouteData<string>("action");
            var id = Guid.Parse(context.GetRouteData<string>("id"));
            
            logger.LogInformation("Requesting get Action by Id. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                thingName, actionName, id);
            
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                    thingName, actionName, id);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actionContext))
            {
                logger.LogInformation("Action not found. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                    thingName, actionName, id);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!actionContext.TryGetValue(id, out var actionInfo))
            {
                logger.LogInformation("Action Id not found. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                    thingName, actionName, id);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            logger.LogInformation("Action Id found. [Thing: {name}][Action: {actionName}][Action Id: {id}]", 
                thingName, actionName, id);
            
            var option = service.GetRequiredService<ThingOption>();
            var namePolicy = option.PropertyNamingPolicy;
            
            await context.WriteBodyAsync(HttpStatusCode.OK, new Dictionary<string, object>
                {
                    [namePolicy.ConvertName(actionName)] = actionInfo
                })
                .ConfigureAwait(false);
        }
    }
}
