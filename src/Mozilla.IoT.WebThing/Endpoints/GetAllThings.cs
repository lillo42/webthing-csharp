using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetAllThings
    {
        internal static Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetAllThings>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            logger.LogInformation("Found {counter} things", things.Count());
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = Const.ContentType;

            return JsonSerializer.SerializeAsync(context.Response.Body, 
                things.Select(thing => thing.ThingContext.Response).ToList(), 
                service.GetRequiredService<ThingOption>().ToJsonSerializerOptions(), 
                context.RequestAborted);
        }
    }
}
