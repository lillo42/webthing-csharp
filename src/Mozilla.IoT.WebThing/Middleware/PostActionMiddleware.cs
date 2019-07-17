using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PostActionMiddleware : AbstractThingMiddleware
    {
        public PostActionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<PostActionMiddleware>(), thingType)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (thing == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var json = await httpContext.ReadBodyAsync<IDictionary<string, object>>();

            if (json == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            string name = httpContext.GetValueFromRoute<string>("actionName");

            if (!json.Keys.Any()) 
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }
            
            var response = new Dictionary<string, object>();
            
            if (json.TryGetValue(name, out var token))
            {
                object input = GetInput(token);
                
                var action = await thing.PerformActionAsync(name, input, httpContext.RequestServices.GetService<IJsonConvert>(), httpContext.RequestAborted)
                    .ConfigureAwait(false);
                
                if (action != null)
                {
                    response.Add(name, action.AsActionDescription());
                    
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
