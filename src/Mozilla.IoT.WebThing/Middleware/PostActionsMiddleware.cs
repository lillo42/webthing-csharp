using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PostActionsMiddleware : AbstractThingMiddleware
    {
        public PostActionsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things)
            : base(next, loggerFactory.CreateLogger<PostActionsMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (thing == null)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var json = await httpContext.ReadBodyAsync<IDictionary<string, object>>();

            if (!json.Keys.Any())
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var response = new Dictionary<string, object>();
            var descriptor = httpContext.RequestServices.GetService<IDescription<Action>>();
            var target = httpContext.RequestServices.GetService<ITargetBlock<Action>>();
            IActionActivator activator = httpContext.RequestServices.GetService<IActionActivator>();

            foreach ((string key, object token) in json)
            {
                object input = GetInput(token);

                Action action = activator.CreateInstance(httpContext.RequestServices, thing, key,
                    input as IDictionary<string, object>);

                if (action != null)
                {
                    thing.Actions.Add(action);
                    IDictionary<string, object> actionDescriptor = descriptor.CreateDescription(action);
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
