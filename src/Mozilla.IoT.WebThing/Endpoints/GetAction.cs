using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
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
            
            logger.LogInformation("Requesting Action for Thing. [Thing: {name}]", thingName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var option = service.GetRequiredService<JsonSerializerOptions>();
            
            var actionName = context.GetRouteData<string>("action");

            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actionContext))
            {
                logger.LogInformation("{action} action not found. [Thing: {name}]", actionName, thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            logger.LogInformation("{action} action found. [Thing: {name}]", actionName, thingName);
            await context.WriteBodyAsync(HttpStatusCode.OK, actionContext)
                .ConfigureAwait(false);
        }
    }
}
