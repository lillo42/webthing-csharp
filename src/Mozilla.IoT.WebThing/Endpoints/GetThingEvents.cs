using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetThingEvents
    {
        public static Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetThingEvents>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            var name = context.GetRouteData<string>("name");
            logger.LogInformation("Requesting Thing. [Name: {name}]", name);
            var thing = things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", name);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }
            
            if (thing.Prefix == null)
            {
                logger.LogDebug("Thing without prefix. [Name: {name}]", thing.Name);
                thing.Prefix = new Uri(UriHelper.BuildAbsolute(context.Request.Scheme, 
                    context.Request.Host));
            }

            var option = service.GetRequiredService<JsonSerializerOptions>();

            var result = new LinkedList<object>();

            try
            {
                foreach (var (key, events) in thing.ThingContext.Events)
                {
                    var @eventsArray = events.ToArray();
                    foreach (var @event in eventsArray)
                    {
                        result.AddLast(new Dictionary<string, object> {[key] = @event});
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            
            logger.LogInformation("Found Thing with {counter} events. [Name: {name}]", result.Count, thing.Name);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = Const.ContentType;
            return JsonSerializer.SerializeAsync(context.Response.Body, result, option);
        }
    }
}
