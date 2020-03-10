using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Converts;

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
            
            logger.LogInformation("Requesting Action for Thing. [Thing: {name}]", thingName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var option = service.GetRequiredService<JsonSerializerOptions>();
            
            var result = new LinkedList<object>();

            foreach (var actions in thing.ThingContext.Actions)
            {
                foreach (var value in actions.Value)
                {
                    result.AddLast(new Dictionary<string, object> {[actions.Key] = value});
                }
            }

            logger.LogInformation("Found {counter} Actions. [Thing: {name}]", result.Count, thingName);
            await context.WriteBodyAsync(HttpStatusCode.OK, result, option)
                .ConfigureAwait(false);
        }
    }
}
