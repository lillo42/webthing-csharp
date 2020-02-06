using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class PutThingProperty
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<PutThingProperty>>();
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
            
            context.Request.EnableBuffering();
            var option = service.GetRequiredService<JsonSerializerOptions>();
            logger.LogTrace("Going to set property {propertyName}", property);
            var json = await context.FromBodyAsync<Dictionary<string, object>>(option)
                .ConfigureAwait(false); 
            var result = thing.ThingContext.Properties.SetProperty(property, json[property]);
            
            if (result == SetPropertyResult.NotFound)
            {
                logger.LogInformation("Property not found. [Thing Name: {thingName}][Property Name: {propertyName}]");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            if (result == SetPropertyResult.InvalidValue)
            {
                logger.LogInformation("Property with Invalid. [Thing Name: {thingName}][Property Name: {propertyName}]");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            await context.WriteBodyAsync(HttpStatusCode.OK, thing.ThingContext.Properties.GetProperties(property), option)
                .ConfigureAwait(false);
        }
    }
}
