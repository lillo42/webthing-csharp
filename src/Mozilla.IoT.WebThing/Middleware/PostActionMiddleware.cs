using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Json;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PostActionMiddleware : AbstractThingMiddleware
    {
        public PostActionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things) 
            : base(next, loggerFactory.CreateLogger<PostActionMiddleware>(), things)
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

            if (!json.Keys.Any()) 
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }

            var response = new Dictionary<string, object>();
            string name = httpContext.GetValueFromRoute<string>("actionName");
            if (thing.ActionsTypes.ContainsKey(name) && json.TryGetValue(name, out var token))
            {
                object input = GetInput(token);
                Action action = thing.GetAction(name, input as IDictionary<string, object>, httpContext.RequestServices);
                
                if (thing.Subscribers.Any())
                {
                    var message = new Dictionary<string, object>
                    {
                        [INPUT] = action.Input,
                        [HREF] = action.HrefPrefix.JoinUrl(action.Href),
                        [STATUS] = action.Status.ToString().ToLower()
                    };

                   await thing.NotifySubscribersAsync(message, httpContext.RequestServices.GetService<IJsonConvert>(),
                       httpContext.RequestServices.GetService<IJsonSerializerSettings>(), httpContext.RequestAborted);
                }

                var descriptor = httpContext.RequestServices.GetService<IDescription<Action>>();
                response.Add(name, descriptor.CreateDescription(action));
                var block = httpContext.RequestServices.GetService<ITargetBlock<Action>>();
                await block.SendAsync(action);
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
