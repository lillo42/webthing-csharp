using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Extensions;

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
            logger.LogInformation("Requesting Action for Thing. [Name: {name}]", thingName);
            var thing = things.FirstOrDefault(x => x.Name.Equals(thingName, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var jsonOption = service.GetRequiredService<JsonSerializerOptions>();
            var option = service.GetRequiredService<ThingOption>();

            var actionName = context.GetRouteData<string>("action");

            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actions))
            {
                logger.LogInformation("{actionName} Action not found in {thingName}", actionName, thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var jsonActions = await context.FromBodyAsync<JsonElement>(jsonOption)
                .ConfigureAwait(false);

            var actionsToExecute = new LinkedList<ActionInfo>();
            
            foreach (var property in jsonActions.EnumerateObject())
            {
                if (!property.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.LogInformation("Invalid {actionName} action. [Thing: {thingName}]", actions, thingName);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                if (actions.TryAdd(property.Value, out var action))
                {
                    logger.LogInformation("{actionName} Action has invalid parameters. [Name: {thingName}]", actions, thingName);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                action.Thing = thing;
                var namePolicy = option.PropertyNamingPolicy;
                action.Href = $"/things/{namePolicy.ConvertName(thing.Name)}/actions/{namePolicy.ConvertName(actionName)}/{action.GetActionName()}";
                actionsToExecute.AddLast(action);
            }
            
            foreach (var action in actionsToExecute)
            {
                logger.LogInformation("Going to execute {actionName} action with {id} Id. [Name: {thingName}]", action.GetActionName(), action.GetId(), thingName);
                
                _ = action.ExecuteAsync(thing, service)
                    .ConfigureAwait(false);
            }
            
            if (actionsToExecute.Count == 1)
            {
                await context.WriteBodyAsync(HttpStatusCode.Created, actionsToExecute.First.Value, jsonOption)
                    .ConfigureAwait(false);
            }
            else
            {
                await context.WriteBodyAsync(HttpStatusCode.Created, actionsToExecute, jsonOption)
                    .ConfigureAwait(false);
            }
        }
    }
}
