using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Middlewares;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetThing
    {
        internal static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetThing>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var name = context.GetRouteData<string>("name");
            logger.LogInformation("Requesting Thing. [Thing: {name}]", name);

            var thing = things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Thing: {name}]", name);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            ThingAdapter.Adapt(context, thing);
            logger.LogInformation("Found 1 Thing. [Thing: {name}]", thing.Name);
            await context.WriteBodyAsync(HttpStatusCode.OK, thing.ThingContext.Response)
                .ConfigureAwait(false);
        }
    }
}
