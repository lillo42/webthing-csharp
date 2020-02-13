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
    internal class PutProperty
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<PutProperty>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var name = context.GetRouteData<string>("name");
            logger.LogInformation("Requesting Thing. [Name: {name}]", name);
            var thing = things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", name);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var property = context.GetRouteData<string>("property");
            
            logger.LogInformation("Going to set property {propertyName}", property);
            
            var json = await context.FromBodyAsync<JsonElement>(new JsonSerializerOptions())
                .ConfigureAwait(false);
            
            var result = thing.ThingContext.Properties.SetProperty(property, json.GetProperty(property));
            
            if (result == SetPropertyResult.NotFound)
            {
                logger.LogInformation("Property not found. [Thing Name: {thingName}][Property Name: {propertyName}]", thing.Name, property);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            if (result == SetPropertyResult.InvalidValue)
            {
                logger.LogInformation("Property with Invalid. [Thing Name: {thingName}][Property Name: {propertyName}]", thing.Name, property);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            await context.WriteBodyAsync(HttpStatusCode.OK, thing.ThingContext.Properties.GetProperties(property), ThingConverter.Options)
                .ConfigureAwait(false);
        }
    }
}
