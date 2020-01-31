using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetThingActionById
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetThingActionById>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            var thingName = context.GetRouteData<string>("name");
            logger.LogInformation("Requesting Action for Thing. [Name: {name}]", thingName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            if (thing.Prefix == null)
            {
                logger.LogDebug("Thing without prefix. [Name: {name}]", thingName);
                thing.Prefix = new Uri(UriHelper.BuildAbsolute(context.Request.Scheme, 
                    context.Request.Host));
            }

            context.Request.EnableBuffering();
            var option = service.GetRequiredService<JsonSerializerOptions>();
            
            var actionName = context.GetRouteData<string>("action");
            var id = context.GetRouteData<Guid>("id");

            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actionContext))
            {
                logger.LogInformation("{actionName} Action not found in {thingName}", actionName, thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var actionInfo = actionContext.Actions.FirstOrDefault(x => x.Id == id);
            if (actionInfo == null)
            {
                logger.LogInformation("{actionName} Action with {id} id not found in {thingName}", actionName, id, thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            await context.WriteBodyAsync(HttpStatusCode.OK, actionInfo, option)
                .ConfigureAwait(false);
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