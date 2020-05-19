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
            logger.LogInformation("Requesting Thing. [Name: {name}]", thingName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var converter = service.GetRequiredService<IJsonConvert>();
            var receivedAction = converter
                .Deserialize<Dictionary<string, object>>(await context.GetBody()
                    .ConfigureAwait(false));

            if (receivedAction.Count != 1)
            {
                logger.LogInformation("accepted only 1 action by executing. [Thing: {thingName}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var (actionName, actionValue) = receivedAction.First();
            
            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var action))
            {
                logger.LogInformation("{actionName} Action not found in {thingName}", actionName, thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!action.TryAdd(actionValue!, out var actionInformation))
            {
                logger.LogInformation("{actionName} Action has invalid parameters. [Name: {thingName}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            
            var option = service.GetRequiredService<ThingOption>().ToJsonSerializerOptions();
            
            actionInformation.Thing = thing;
            var namePolicy = option.PropertyNamingPolicy;
            actionInformation.Href = $"/things/{namePolicy.ConvertName(thing.Name)}/actions/{namePolicy.ConvertName(actionInformation.GetActionName())}/{actionInformation.GetId()}";
            
            logger.LogInformation("Going to execute {actionName} action. [Name: {thingName}]", actionInformation.GetActionName(), thingName);
              _ = actionInformation.ExecuteAsync(thing, service).ConfigureAwait(false);
              
            await context.WriteBodyAsync(HttpStatusCode.Created, actionInformation)
                .ConfigureAwait(false);
        }
    }
}
