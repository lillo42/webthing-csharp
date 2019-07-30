using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetActionByIdMiddleware : AbstractThingMiddleware
    {
        public GetActionByIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory,
            IThingReadOnlyCollection things)
            : base(next, loggerFactory.CreateLogger<GetActionByIdMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Logger.LogInformation("Get Actions is calling");

            var thingId = httpContext.GetValueFromRoute<string>("thing");
            var name = httpContext.GetValueFromRoute<string>("actionName");
            var id = httpContext.GetValueFromRoute<string>("actionId");

            var thing = Things[thingId];

            Logger.LogInformation("Get action: [" +
                                  $"[thing: {thingId}]" +
                                  $"[actionId: {id}]" +
                                  $"[actionName: {name}]]");

            if (thing != null && thing.Actions.Contains(name))
            {
                var action = thing.Actions[name].FirstOrDefault(x => x.Id == id);
                if (action != null)
                {
                    var description = httpContext.RequestServices.GetService<IDescription<Action>>();
                    await httpContext.WriteBodyAsync(HttpStatusCode.OK,
                        new Dictionary<string, object> {[name] = description.CreateDescription(action)});
                    return;
                }
            }

            Logger.LogInformation(
                $"Thing or Action not found. [[thing: {thingId}][actionId: {id}][actionName: {name}]]");

            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
