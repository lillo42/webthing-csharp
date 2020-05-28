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
    internal class GetAllThings
    {
        internal static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetAllThings>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            logger.LogInformation("Request all things.");
            await context.WriteBodyAsync(HttpStatusCode.OK, things.Select(thing =>
            {
                ThingAdapter.Adapt(context, thing);
                return thing.ThingContext.Response;
            })).ConfigureAwait(false);
        }
    }
}
