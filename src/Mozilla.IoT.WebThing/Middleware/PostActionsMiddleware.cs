using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PostActionsMiddleware : AbstractThingMiddleware
    {
        public PostActionsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType)
            : base(next, loggerFactory.CreateLogger<PostActionsMiddleware>(), thingType)
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

            foreach ((string key, object token) in json)
            {
                object input = GetInput(token);
                
                Action action = await thing.PerformActionAsync(key, input, httpContext.RequestAborted);

                if (action != null)
                {
                    response.Add(key, action.AsActionDescription());
                    var target = httpContext.RequestServices.GetService<ITargetBlock<Action>>();
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
