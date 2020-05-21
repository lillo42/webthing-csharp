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
    internal class GetProperties
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetProperties>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var name = context.GetRouteData<string>("name");
            
            logger.LogInformation("Requesting get all properties. [Thing: {name}]", name);
            var thing = things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}]", name);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            logger.LogInformation("Going to get all properties [Thing: {name}]", name);

            var properties = new Dictionary<string, object?>();
            
            foreach (var (propertyName, property) in thing.ThingContext.Properties)
            {
                if (property.TryGetValue(out var value))
                {
                    properties.Add(propertyName, value);
                }
            }
            
            await context.WriteBodyAsync(HttpStatusCode.OK, properties)
                .ConfigureAwait(false);
        }
    }
}
