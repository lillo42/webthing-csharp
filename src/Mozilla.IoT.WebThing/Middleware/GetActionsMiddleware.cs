using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetActionsMiddleware : AbstractThingMiddleware
    {
        public GetActionsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things)
            : base(next, loggerFactory.CreateLogger<GetActionsMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Logger.LogInformation("Get Actions is calling");
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            
            Logger.LogInformation($"Get Actions: [[thing: {thingId}]]");
            var thing = Things[thingId];

            if (thing == null)
            {
                Logger.LogInformation($"Get Action: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var description = httpContext.RequestServices.GetService<IDescription<Action>>();


            var result = new LinkedList<IDictionary<string,object>>();

            foreach ((string name, ICollection<Action> actions) in thing.Actions)
            {
                foreach (var action in actions)
                {
                    result.AddFirst(new Dictionary<string, object>
                    {
                        [name] = description.CreateDescription(action)
                    });
                }
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, result);
        }
    }
}
