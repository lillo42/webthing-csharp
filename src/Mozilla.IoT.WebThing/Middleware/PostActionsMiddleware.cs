using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PostActionsMiddleware : AbstractThingMiddleware
    {
        private static readonly JsonSerializer s_serializer = new JsonSerializer();
        
        public PostActionsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<PostActionsMiddleware>(), thingType)
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

            var response = new JObject();
            
            foreach (KeyValuePair<string,JToken> token in json)
            {
                Action action = await thing.PerformActionAsync(token.Key, 
                    (JObject)token.Value["input"],
                    httpContext.RequestAborted);

                if (action != null)
                {
                    response.Add(token.Key, action.AsActionDescription());

                    action.StartAsync(CancellationToken.None)
                        .ConfigureAwait(false);
                }
            }
            
            await httpContext.WriteBodyAsync(HttpStatusCode.Created, response);
        }
    }
}
