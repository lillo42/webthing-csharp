using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetEvents
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetEvents>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var name = context.GetRouteData<string>("name");
            logger.LogInformation("Requesting Events. [Thing: {name}]", name);
            
            var thing = things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}]", name);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var option = service.GetRequiredService<ThingOption>();
            var result = new LinkedList<Dictionary<string, object>>();
            
            foreach (var (key, events) in thing.ThingContext.Events)
            {
                var @eventsArray = events.ToArray();
                foreach (var @event in eventsArray)
                {
                    result.AddLast(new Dictionary<string, object>
                    {
                        [option.PropertyNamingPolicy.ConvertName(key)] = @event
                    });
                }
            }
            
            logger.LogInformation("Found {counter} events. [Thing: {name}]", result.Count, thing.Name);
            await context.WriteBodyAsync(HttpStatusCode.OK, result);
        }
    }
}
