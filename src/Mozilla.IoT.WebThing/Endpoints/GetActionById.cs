using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal static class GetActionById
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetService<ILogger>();
            logger.LogInformation("Get Action by Id is calling");

            var thingId = httpContext.GetValueFromRoute<string>("thing");
            var name = httpContext.GetValueFromRoute<string>("name");
            var id = httpContext.GetValueFromRoute<string>("id");

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
                    var description = httpContext.RequestServices.GetService<IDescription<Action>>();
                    await httpContext.WriteBodyAsync(HttpStatusCode.OK,
                        new Dictionary<string, object>
                        {
                            [name] = description.CreateDescription(action)
                        });
                    return;
                }
            }

            logger.LogInformation(
                $"Get Action by Id: Thing or Action not found. [[thing: {thingId}][actionId: {id}][actionName: {name}]]");

            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
