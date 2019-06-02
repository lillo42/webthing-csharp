using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PostActionMiddleware : AbstractThingMiddleware
    {
        private static readonly JsonSerializer s_serializer = new JsonSerializer();
        
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
            
            JObject json = await httpContext.ReadBodyAsync<JObject>()
                .ConfigureAwait(false);

            if (json == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            string name = httpContext.GetValueFromRoute<string>("actionName");

            IEnumerable<JProperty> properties = json.Properties();

            if (properties == null || !properties.Any())
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }
            
            var response = new JObject();
            
            if (json.TryGetValue(name, out JToken token))
            {
                JObject input = token.Contains("input") ? (JObject)token["input"] : new JObject();
                var action = await thing.PerformActionAsync(name, input, httpContext.RequestAborted)
                    .ConfigureAwait(false);
                
                if (action != null)
                {
                    response.Add(name, action.AsActionDescription());
                    
                    var block = httpContext.RequestServices.GetService<ITargetBlock<Action>>();
                
                    await block.SendAsync(action)
                        .ConfigureAwait(false);
                }
            }
            
            await httpContext.WriteBodyAsync(HttpStatusCode.Created, response)
                .ConfigureAwait(false);
        }
    }
}
