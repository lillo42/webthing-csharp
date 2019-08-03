using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal static  class PostActions
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetService<ILogger>();
            
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            logger.LogInformation($"Post Actions is calling: [[thing: {thingId}]");
            
            var thing = services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);

            if (thing == null)
            {
                logger.LogInformation($"Post Actions: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var json = await httpContext.ReadBodyAsync<IDictionary<string, object>>();
            
            if (json == null)
            {
                logger.LogInformation("Post Action: Body not found");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            if (!json.Keys.Any())
            {
                logger.LogInformation("Post Action: Body is empty");
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var response = new Dictionary<string, object>();
            var descriptor = services.GetService<IDescriptor<Action>>();
            var target = services.GetService<ITargetBlock<Action>>();
            var activator = services.GetService<IActionActivator>();

            foreach ((string key, object token) in json)
            {
                var input = GetInput(token);

                Action action = activator.CreateInstance(httpContext.RequestServices, thing, key,
                    input as IDictionary<string, object>);

                if (action != null)
                {
                    thing.Actions.Add(action);
                    var actionDescriptor = descriptor.CreateDescription(action);
                    response.Add(key, actionDescriptor);
                    await target.SendAsync(action);
                }
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.Created, response);
        }

        private static object GetInput(object token)
        {
            if (token is IDictionary<string, object> dictionary && dictionary.ContainsKey("input"))
            {
                return dictionary["input"];
            }

            return new object();
        }
    }
}
