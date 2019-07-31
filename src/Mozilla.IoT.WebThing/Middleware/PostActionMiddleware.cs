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
    public class PostActionMiddleware : AbstractThingMiddleware
    {
        public PostActionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things) 
            : base(next, loggerFactory.CreateLogger<PostActionMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            Logger.LogInformation($"Post Action is calling: [[thing: {thingId}]");
            
            var thing = Things[thingId];

            if (thing == null)
            {
                Logger.LogInformation($"Post Action: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
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
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }

            var response = new Dictionary<string, object>();
            var name = httpContext.GetValueFromRoute<string>("actionName");
            if (thing.ActionsTypeInfo.ContainsKey(name) && json.TryGetValue(name, out var token))
            {
                var input = GetInput(token);
                var activator = httpContext.RequestServices.GetService<IActionActivator>();
                
                Action action = activator.CreateInstance(httpContext.RequestServices, 
                    thing, name, input as IDictionary<string, object>);

                if (action != null)
                {
                    thing.Actions.Add(action);
                    var descriptor = httpContext.RequestServices.GetService<IDescription<Action>>();
                    response.Add(name, descriptor.CreateDescription(action));
                    var block = httpContext.RequestServices.GetService<ITargetBlock<Action>>();
                    await block.SendAsync(action);    
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
