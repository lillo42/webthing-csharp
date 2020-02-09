using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class PostThingActions
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<PostThingActions>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            var thingName = context.GetRouteData<string>("name");
            logger.LogInformation("Requesting Thing. [Name: {name}]", thingName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            context.Request.EnableBuffering();
            var option = service.GetRequiredService<JsonSerializerOptions>();
            
            var actions =  await context.FromBodyAsync<Dictionary<string, JsonElement>>(option)
                .ConfigureAwait(false);
            
            var actionsToExecute = new LinkedList<ActionInfo>();
            foreach (var (actionName, json) in actions)
            {
                if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actionContext))
                {
                    logger.LogInformation("{actionName} Action not found in {thingName}", actions, thingName);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                
                logger.LogTrace("{actionName} Action found. [Name: {thingName}]", actions, thingName);
                var action = (ActionInfo)JsonSerializer.Deserialize(json.GetRawText(),
                    actionContext.ActionType, option);
                
                if (!action.IsValid())
                {
                    logger.LogInformation("{actionName} Action has invalid parameters. [Name: {thingName}]", actions, thingName);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                actionsToExecute.AddLast(action); 
                action.Thing = thing;
                actionContext.Actions.Add(action);
            }
            
            foreach (var actionInfo in actionsToExecute)
            {
                logger.LogInformation("Going to execute actions. [Name: {thingName}]", thingName);
                actionInfo.ExecuteAsync(thing, service)
                    .ConfigureAwait(false);
            }
            
            if (actionsToExecute.Count == 1)
            {
                await context.WriteBodyAsync(HttpStatusCode.Created, actionsToExecute.First, option)
                    .ConfigureAwait(false);
            }
            else
            {
                await context.WriteBodyAsync(HttpStatusCode.Created, actionsToExecute, option)
                    .ConfigureAwait(false);
            }
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
