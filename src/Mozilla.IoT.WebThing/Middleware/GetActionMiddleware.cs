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
    public class GetActionMiddleware : AbstractThingMiddleware
    {
        public GetActionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things)
            : base(next, loggerFactory.CreateLogger<GetActionMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Logger.LogInformation("Get Action is calling");

            var thingId = httpContext.GetValueFromRoute<string>("thing");
            var name = httpContext.GetValueFromRoute<string>("actionName");

            Logger.LogInformation($"Get Action: [[thing: {thingId}][actionName: {name}]]");
            var thing = Things[thingId];
            
            if (thing != null && thing.Actions.Contains(name))
            {
                var description = httpContext.RequestServices.GetService<IDescription<Action>>();
                var result = thing.Actions[name].ToDictionary(x => x.Name, x => description.CreateDescription(x));
                await httpContext.WriteBodyAsync(HttpStatusCode.OK, result);
            }
            else
            {
                Logger.LogInformation(
                    $"Get Action: Thing or action not found [[thing: {thingId}][actionName: {name}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
    }
}
