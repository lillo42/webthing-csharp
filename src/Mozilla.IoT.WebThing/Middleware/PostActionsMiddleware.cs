using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PostActionsMiddleware : AbstractThingMiddleware
    {
        public PostActionsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things)
            : base(next, loggerFactory.CreateLogger<PostActionsMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            Logger.LogInformation($"Post Actions is calling: [[thing: {thingId}]");
            
            var thing = Things[thingId];

            if (thing == null)
            {
                Logger.LogInformation($"Post Actions: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var json = await httpContext.ReadBodyAsync<IDictionary<string, object>>();
            
            if (json == null)
            {
                Logger.LogInformation("Post Action: Body not found");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            if (!json.Keys.Any())
            {
                Logger.LogInformation("Post Action: Body is empty");
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var response = new Dictionary<string, object>();
            var descriptor = httpContext.RequestServices.GetService<IDescription<Action>>();
            var target = httpContext.RequestServices.GetService<ITargetBlock<Action>>();
            var activator = httpContext.RequestServices.GetService<IActionActivator>();

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
