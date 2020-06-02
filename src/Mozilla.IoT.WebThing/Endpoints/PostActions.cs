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
    internal class PostActions
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<PostActions>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var thingName = context.GetRouteData<string>("name");
            logger.LogInformation("Requesting Post Actions. [Thing: {name}]", thingName);
            
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var converter = service.GetRequiredService<IJsonConvert>();
            var receivedAction = converter
                .Deserialize<Dictionary<string, object>>(await context.GetBody()
                    .ConfigureAwait(false));

            if (receivedAction.Count != 1)
            {
                logger.LogInformation("Accepted only 1 action by executing. [Thing: {thingName}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var (actionName, actionValue) = receivedAction.First();
            
            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var action))
            {
                logger.LogInformation("Action not found. [Thing: {name}][Action: {actionName}]", 
                    thingName, actionName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!action.TryAdd(actionValue!, out var actionInformation))
            {
                logger.LogInformation("Action has invalid parameters. [Thing: {name}][Action: {actionName}]", 
                    thingName, actionName);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            
            var option = service.GetRequiredService<ThingOption>();
            var namePolicy = option.PropertyNamingPolicy;
            
            actionInformation.Thing = thing;
            actionInformation.Href = $"/things/{namePolicy.ConvertName(thing.Name)}/actions/{namePolicy.ConvertName(actionInformation.GetActionName())}/{actionInformation.GetId()}";
            
            logger.LogInformation("Going to execute action. [Thing: {name}][Action: {actionName}][Action Id: {actionId}]", 
                thingName, actionName, actionInformation.GetId());

            await context.WriteBodyAsync(HttpStatusCode.Created, new Dictionary<string, object>
                {
                    [namePolicy.ConvertName(actionName)] = actionInformation
                })
                .ConfigureAwait(false);
            
            _ = actionInformation.ExecuteAsync(thing, service).ConfigureAwait(false);
        }
    }
}
