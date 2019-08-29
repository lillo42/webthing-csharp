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
    internal sealed class GetAction
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<GetAction>>();
            logger.LogInformation("Get Action is calling");

            var thingId = httpContext.GetValueFromRoute<string>("thing");
            var name = httpContext.GetValueFromRoute<string>("name");

            logger.LogInformation($"Get Action: [[thing: {thingId}][actionName: {name}]]");
            var thing = services.GetService<IThingActivator>().CreateInstance(services, thingId);;
            
            if (thing != null && thing.Actions.Contains(name))
            {
                var description = services.GetService<IDescriptor<Action>>();
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
