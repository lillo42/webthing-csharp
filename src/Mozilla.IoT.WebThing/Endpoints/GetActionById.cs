using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Activator;
using Mozilla.IoT.WebThing.Descriptor;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal sealed class GetActionById
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<GetActionById>>();
            logger.LogInformation("Get Action by Id is calling");

            var route = services.GetRequiredService<IHttpRouteValue>();
            var thingId = route.GetValue<string>("thing");
            var name = route.GetValue<string>("name");
            var id = route.GetValue<string>("id");

            var thing = services.GetService<IThingActivator>().CreateInstance(services, thingId);

            logger.LogInformation("Get Action by Id: [" +
                                  $"[thing: {thingId}]" +
                                  $"[actionId: {id}]" +
                                  $"[actionName: {name}]]");

            if (thing != null && thing.Actions.Contains(name))
            {
                var action = thing.Actions[name].FirstOrDefault(x => x.Id == id);
                if (action != null)
                {
                    var description = httpContext.RequestServices.GetRequiredService<IDescriptor<Action>>();
                    var writer = services.GetRequiredService<IHttpBodyWriter>();
                    httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    
                    await writer.WriteAsync(new Dictionary<string, object>
                        {
                            [name] = description.CreateDescription(action)
                        }, httpContext.RequestAborted);
                    return;
                }
            }

            logger.LogInformation(
                $"Get Action by Id: Thing or Action not found. [[thing: {thingId}][actionId: {id}][actionName: {name}]]");

            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
