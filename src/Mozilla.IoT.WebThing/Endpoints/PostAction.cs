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
            
            var actions = await context.FromBodyAsync<Dictionary<string, JsonElement>>(jsonOption)
                .ConfigureAwait(false);
            
            var actionName = context.GetRouteData<string>("action");

            if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actionContext))
            {
                logger.LogInformation("{actionName} Action not found in {thingName}", actions, thingName);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            if (actions.Keys.Any(x => x != actionName))
            {
                logger.LogInformation("Payload has invalid action. [Name: {thingName}][Action Name: {actionName}]", thingName, actionName);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            
            var actionsToExecute = new LinkedList<ActionInfo>();
            
            foreach (var (_, json) in actions)
            {
                logger.LogTrace("{actionName} Action found. [Name: {thingName}]", actions, thingName);
                var action = (ActionInfo)JsonSerializer.Deserialize(json.GetRawText(),
                    actionContext.ActionType, jsonOption);
                
                if (!action.IsValid())
                {
                    logger.LogInformation("{actionName} Action has invalid parameters. [Name: {thingName}]", actions, thingName);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                action.Thing = thing;
                var namePolicy = option.PropertyNamingPolicy;
                action.Href = $"/things/{namePolicy.ConvertName(thing.Name)}/actions/{namePolicy.ConvertName(actionName)}/{action.Id}";
                actionsToExecute.AddLast(action);
            }
            
            foreach (var actionInfo in actionsToExecute)
            {
                logger.LogInformation("Going to execute action {actionName}. [Name: {thingName}]", actionName, thingName);
                
                actionInfo.ExecuteAsync(thing, service)
                    .ConfigureAwait(false);
                
                actionContext.Actions.Add(actionInfo.Id, actionInfo);
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
