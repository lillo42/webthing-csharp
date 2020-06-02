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
    internal class GetEvent
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetEvent>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var name = context.GetRouteData<string>("name");
            var @event = context.GetRouteData<string>("event");
            
            logger.LogInformation("Requesting Thing event. [Thing: {name}][Event: {eventName}]", name, @event);
            
            var thing = things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}][Event: {eventName}]", name, @event);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!thing.ThingContext.Events.TryGetValue(@event, out var events))
            {
                logger.LogInformation("Event not found.[Thing: {thingName}][Event: {eventName}]", thing.Name, @event);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var option = service.GetRequiredService<ThingOption>();
            var result = new LinkedList<Dictionary<string, object>>();

            foreach (var e in events.ToArray())
            {
                result.AddLast(new Dictionary<string, object>
                {
                    [option.PropertyNamingPolicy.ConvertName(@event)] = e
                });
            }

            logger.LogInformation("Found {counter} events. [Thing: {name}][Event: {eventName}]", result.Count, thing.Name, @event);
            await context.WriteBodyAsync(HttpStatusCode.OK, result);
        }
    }
}
