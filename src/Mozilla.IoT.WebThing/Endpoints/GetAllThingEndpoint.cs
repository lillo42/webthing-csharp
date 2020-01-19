using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetAllThingEndpoint
    {
        internal static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetAllThingEndpoint>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            var option = service.GetRequiredService<JsonSerializerOptions>();
            
            logger.LogTrace("Found {counter} things", things.Count());
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await JsonSerializer.SerializeAsync(context.Response.Body, things, option);
        }
    }
}
