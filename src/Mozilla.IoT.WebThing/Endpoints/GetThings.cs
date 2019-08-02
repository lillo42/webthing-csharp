using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Builder;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal static class GetThings
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetService<ILogger>();
            
            logger.LogInformation("Get Things is calling");

            var array = new LinkedList<IDictionary<string, object>>();
            var descriptor = services.GetService<IDescription<Thing>>();
            var builder = services.GetService<IWsUrlBuilder>();
            var things = services.GetService<IThingActivator>();
            
            foreach (var thing in things)
            {
                var description = descriptor.CreateDescription(thing);

                var link = new Dictionary<string, object>
                {
                    ["rel"] = "alternate",
                    ["href"] = builder.Build(httpContext.Request, thing.Name)
                };
                
                ((ICollection<IDictionary<string, object>>)description["links"]).Add(link);
                array.AddLast(description);
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, array);
        }
    }
}
