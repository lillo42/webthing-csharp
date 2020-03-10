using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            
            var jsonAction =  await context.FromBodyAsync<JsonElement>(jsonOption)
                .ConfigureAwait(false);
            
            var actionsToExecute = new LinkedList<ActionInfo>();

            foreach (var property in jsonAction.EnumerateObject())
            {
                if (!thing.ThingContext.Actions.TryGetValue(property.Name, out var actions))
                {
                    logger.LogInformation("{actionName} Action not found in {thingName}", actions, thingName);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                var action = actions.Add(property.Value);
                
                if (action == null)
                {
                    logger.LogInformation("{actionName} Action has invalid parameters. [Name: {thingName}]", actions, thingName);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }
                
                action.Thing = thing;
                var namePolicy = option.PropertyNamingPolicy;
                action.Href = $"/things/{namePolicy.ConvertName(thing.Name)}/actions/{namePolicy.ConvertName(action.GetActionName())}/{action.GetId()}";

                actionsToExecute.AddLast(action);

            }
            
            foreach (var actionInfo in actionsToExecute)
            {
                logger.LogInformation("Going to execute {actionName} action. [Name: {thingName}]", actionInfo.GetActionName(), thingName);
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
