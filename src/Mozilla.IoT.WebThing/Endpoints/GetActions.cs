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
    internal class GetActions
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetActions>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            var thingName = context.GetRouteData<string>("name");
            
            logger.LogInformation("Requesting Get all Actions for Thing. [Thing: {name}]", thingName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var result = new LinkedList<object>();

            foreach (var (key, actionCollection) in thing.ThingContext.Actions)
            {
                foreach (var value in actionCollection)
                {
                    result.AddLast(new Dictionary<string, object> {[key] = value});
                }
            }

            logger.LogInformation("Found {counter} Actions. [Thing: {name}]", result.Count, thingName);
            await context.WriteBodyAsync(HttpStatusCode.OK, result)
                .ConfigureAwait(false);
        }
    }
}
