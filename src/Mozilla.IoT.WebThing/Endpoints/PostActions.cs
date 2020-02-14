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
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Extensions;

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
            
            context.Request.EnableBuffering();
            var jsonOption = service.GetRequiredService<JsonSerializerOptions>();
            var option = service.GetRequiredService<ThingOption>();
            
            var actions =  await context.FromBodyAsync<Dictionary<string, JsonElement>>(jsonOption)
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
                    actionContext.ActionType, jsonOption);
                
                if (!action.IsValid())
                {
                    logger.LogInformation("{actionName} Action has invalid parameters. [Name: {thingName}]", actions, thingName);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                actionsToExecute.AddLast(action); 
                action.Thing = thing;
                var namePolicy = option.PropertyNamingPolicy;
                action.Href = $"/things/{namePolicy.ConvertName(thing.Name)}/actions/{namePolicy.ConvertName(actionName)}/{action.Id}";
            }
            
            foreach (var actionInfo in actionsToExecute)
            {
                logger.LogInformation("Going to execute {actionName} action. [Name: {thingName}]", actionInfo.GetActionName(), thingName);
                
                thing.ThingContext.Actions[actionInfo.GetActionName()].Actions.Add(actionInfo.Id, actionInfo);
                
                actionInfo.ExecuteAsync(thing, service)
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
