using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Converts;

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
            logger.LogInformation("Requesting Action for Thing. [Name: {name}]", thingName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var option = ThingConverter.Options;;
            
            var actionName = context.GetRouteData<string>("action");
            var id = Guid.Parse(context.GetRouteData<string>("id"));

            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actionContext))
            {
                logger.LogInformation("{actionName} Action not found in {thingName}", actionName, thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!actionContext.Actions.TryGetValue(id, out var actionInfo))
            {
                logger.LogInformation("{actionName} Action with {id} id not found in {thingName}", actionName, id, thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            await context.WriteBodyAsync(HttpStatusCode.OK, actionInfo, option)
                .ConfigureAwait(false);
        }
    }
}
