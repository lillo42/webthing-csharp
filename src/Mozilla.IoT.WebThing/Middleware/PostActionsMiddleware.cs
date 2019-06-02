using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
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
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            JObject json = await httpContext.ReadBodyAsync<JObject>()
                .ConfigureAwait(false);

            if (json == null || !json.Properties().Any())
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var response = new JObject();

            foreach ((string key, JToken token) in json)
            {
                JObject input = token.Contains("input") ? (JObject)token["input"] : new JObject();
                Action action = await thing.PerformActionAsync(key, input, httpContext.RequestAborted);

                if (action != null)
                {
                    response.Add(key, action.AsActionDescription());
                    var target = httpContext.RequestServices.GetService<ITargetBlock<Action>>();
                    await target.SendAsync(action)
                        .ConfigureAwait(false);
                }
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.Created, response)
                .ConfigureAwait(false);
        }
    }
}
