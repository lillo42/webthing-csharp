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
    internal class PutProperty
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<PutProperty>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var name = context.GetRouteData<string>("name");
            logger.LogInformation("Requesting to change property value. [Name: {name}]", name);
            var thing = things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", name);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var propertyName = context.GetRouteData<string>("property");
            
            logger.LogInformation("Going to set property {propertyName}", propertyName);

            if (!thing.ThingContext.Properties.TryGetValue(propertyName, out var property))
            {
                logger.LogInformation("Property not found. [Thing: {thingName}][Property: {propertyName}]", thing.Name, propertyName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var converter = service.GetRequiredService<IJsonConvert>();

            var jsonProperties =  converter
                .Deserialize<Dictionary<string, object>>(await context.GetBody()
                    .ConfigureAwait(false));

            if (jsonProperties.TryGetValue(propertyName, out var jsonValue))
            {
                switch (property.TrySetValue(jsonValue!))
                {
                    case SetPropertyResult.InvalidValue:
                        logger.LogInformation("Property with Invalid. [Thing Name: {thingName}][Property Name: {propertyName}]", thing.Name, property);
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return;
                    case SetPropertyResult.ReadOnly:
                        logger.LogInformation("Read-Only Property. [Thing Name: {thingName}][Property Name: {propertyName}]", thing.Name, property);
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return;
                    case SetPropertyResult.Ok:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                logger.LogInformation("Invalid property name. [Thing: {thingName}][Excepted property: {propertyName}]", thing.Name, propertyName);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            property.TryGetValue(out var value);
            await context.WriteBodyAsync(HttpStatusCode.OK, new Dictionary<string, object?> {[propertyName] = value})
                .ConfigureAwait(false);
        }
    }
}
