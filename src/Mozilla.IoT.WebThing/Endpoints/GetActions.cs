using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal static class GetActions
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetService<ILogger>();
            logger.LogInformation("Get Action is calling");

            var thingId = httpContext.GetValueFromRoute<string>("thing");
            var name = httpContext.GetValueFromRoute<string>("actionName");

            logger.LogInformation($"Get Action: [[thing: {thingId}][actionName: {name}]]");
            var thing = services.GetService<IThingActivator>().CreateInstance(services, thingId);;
            
            if (thing != null && thing.Actions.Contains(name))
            {
                var description = services.GetService<IDescription<Action>>();
                var result = thing.Actions[name].ToDictionary(x => x.Name, x => description.CreateDescription(x));
                await httpContext.WriteBodyAsync(HttpStatusCode.OK, result);
            }
            else
            {
                logger.LogInformation(
                    $"Get Action: Thing or action not found [[thing: {thingId}][actionName: {name}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
    }
}
