using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class PostAction
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<PostAction>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();

            var thingName = context.GetRouteData<string>("name");
            var actionName = context.GetRouteData<string>("action");

            logger.LogInformation("Requesting Post Action. [Thing: {name}][Action: {actionName}]",
                thingName, actionName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}][Action: {actionName}]",
                    thingName, actionName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actions))
            {
                logger.LogInformation("Action not found. [Thing: {name}][Action: {actionName}]",
                    thingName, actionName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var converter = service.GetRequiredService<IJsonConvert>();

            var jsonActions = converter
                .Deserialize<Dictionary<string, object>>(await context.GetBody()
                    .ConfigureAwait(false));

            if (jsonActions.Count != 1)
            {
                logger.LogInformation("Accepted only 1 action by executing. [Thing: {name}][Action: {actionName}]",
                    thingName, jsonActions);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var (jsonActionName, jsonActionValue) = jsonActions.First();
            
            if (!jsonActionName.Equals(actionName, StringComparison.InvariantCultureIgnoreCase))
            {
                logger.LogInformation("Invalid action. [Thing: {name}][Action: {actionName}]", 
                    thingName, jsonActions);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            
            if (!actions.TryAdd(jsonActionValue, out var action))
            {
                logger.LogInformation("Action has invalid parameters. [Thing: {name}][Action: {actionName}]", 
                    thingName, actionName);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            action.Thing = thing;
            var option = service.GetRequiredService<ThingOption>();
            var namePolicy = option.PropertyNamingPolicy;
            action.Href = $"/things/{namePolicy.ConvertName(thing.Name)}/actions/{namePolicy.ConvertName(actionName)}/{action.GetId()}";

            logger.LogInformation("Action started to execute. [Thing: {name}][Action: {actionName}][Action Id: {actionId}]", 
                thingName, action, action.GetId());
            await context.WriteBodyAsync(HttpStatusCode.Created, new Dictionary<string, object>
                {
                    [namePolicy.ConvertName(actionName)] = action
                })
                .ConfigureAwait(false);
            
            _ = action.ExecuteAsync(thing, service);
        }
    }
}
