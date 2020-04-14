using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetProperty
    {
        public static Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetProperty>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var name = context.GetRouteData<string>("name");
            
            logger.LogInformation("Requesting Thing. [Thing: {name}]", name);
            var thing = things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}]", name);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }

            var propertyName = context.GetRouteData<string>("property");

            if (!thing.ThingContext.Properties.TryGetValue(propertyName, out var property))
            {
                logger.LogInformation("Property not found. [Thing: {thingName}][Property: {propertyName}]", thing.Name, propertyName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }
            
            logger.LogInformation("Found Property. [Thing: {thingName}][Property: {propertyName}]", thing.Name, propertyName);
            if (!property.TryGetValue(out var value))
            {
                logger.LogInformation("The Property is Write-only. [Thing: {thingName}][Property: {propertyName}]", thing.Name, propertyName);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Task.CompletedTask;
            }
            
            context.StatusCodeResult(HttpStatusCode.OK);
            
            var writer = service.GetRequiredService<IJsonWriter>();
            return writer.WriteAsync(new Dictionary<string, object?>
            {
                [propertyName] = value
            });
        }
    }
}
