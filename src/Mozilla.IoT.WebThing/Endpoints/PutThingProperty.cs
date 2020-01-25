using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
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
            
            if (thing.Prefix == null)
            {
                logger.LogDebug("Thing without prefix. [Name: {name}]", thing.Name);
                thing.Prefix = new Uri(UriHelper.BuildAbsolute(context.Request.Scheme, 
                    context.Request.Host));
            }
            
            var property = context.GetRouteData<string>("property");
            
            context.Request.EnableBuffering();
            var option = service.GetRequiredService<JsonSerializerOptions>();
            var jsonString = await GetJsonString(context);
            logger.LogTrace("Going to set property {propertyName} with JSON: {json}", property, jsonString);
            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, option);
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

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = Const.ContentType;
            await JsonSerializer.SerializeAsync(context.Response.Body, thing.ThingContext.Properties.GetProperties(property), option);
        }

        private static async Task<string> GetJsonString(HttpContext context)
        {
            // Build up the request body in a string builder.
            var builder = new StringBuilder();

            // Rent a shared buffer to write the request body into.
            var buffer = ArrayPool<byte>.Shared.Rent(4096);

            while (true)
            {
                var bytesRemaining = await context.Request.Body.ReadAsync(buffer, offset: 0, buffer.Length);
                if (bytesRemaining == 0)
                {
                    break;
                }

                // Append the encoded string into the string builder.
                var encodedString = Encoding.UTF8.GetString(buffer, 0, bytesRemaining);
                builder.Append(encodedString);
            }

            ArrayPool<byte>.Shared.Return(buffer);

            var jsonString = builder.ToString();
            return jsonString;
        }
    }
}
