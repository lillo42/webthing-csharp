using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace WebThing.AspNetCore.Extensions.Middlewares
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
                await NotFoundAsync(httpContext);
                return;
            }
            
            JObject json = ParseBody(httpContext);

            if (json == null)
            {
                await NotFoundAsync(httpContext);
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
            
            await CreatedAsync(httpContext, response);
        }

        private static JObject ParseBody(HttpContext httpContext)
        {
            using (var reader = new BsonDataReader(httpContext.Request.Body))
            {
                return s_serializer.Deserialize<JObject>(reader);
            }
        }

        private static string GetActionName(HttpContext httpContext)
        {
            if (httpContext.GetRouteData().Values.TryGetValue("actionName", out object data))
            {
                return data.ToString();
            }
            
            return null;
        }
    }
}
