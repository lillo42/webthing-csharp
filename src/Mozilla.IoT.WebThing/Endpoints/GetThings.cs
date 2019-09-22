using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Activator;
using Mozilla.IoT.WebThing.Builder;
using Mozilla.IoT.WebThing.Descriptor;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal sealed class GetThings
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<GetThings>>();
            
            logger.LogInformation("Get Things is calling");

            var array = new LinkedList<IDictionary<string, object>>();
            var descriptor = services.GetRequiredService<IDescriptor<Thing>>();
            var builder = services.GetRequiredService<IWsUrlBuilder>();
            var things = services.GetRequiredService<IThingActivator>();
            
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

            var writer = services.GetRequiredService<IHttpBodyWriter>();
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            await writer.WriteAsync(array, httpContext.RequestAborted);
        }
    }
}
