using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal static class GetActions
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetService<ILogger>();
            
            logger.LogInformation("Get Actions is calling");
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            
            logger.LogInformation($"Get Actions: [[thing: {thingId}]]");
            var thing = services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);

            if (thing == null)
            {
                logger.LogInformation($"Get Action: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var description = services.GetService<IDescription<Action>>();


            var result = new LinkedList<IDictionary<string,object>>();

            foreach ((string name, ICollection<Action> actions) in thing.Actions)
            {
                foreach (var action in actions)
                {
                    result.AddFirst(new Dictionary<string, object>
                    {
                        [name] = description.CreateDescription(action)
                    });
                }
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, result);
        }
    }
}
