using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetAllThingEndpoint
    {
        internal static Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetAllThingEndpoint>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            var option = service.GetRequiredService<JsonSerializerOptions>();

            logger.LogDebug("Verify if Things have prefix");
            foreach (var thing in things)
            {
                if (thing.Prefix == null)
                {
                    logger.LogDebug("Thing without prefix. [Name: {name}]", thing.Name);
                    thing.Prefix = new Uri(UriHelper.BuildAbsolute(context.Request.Scheme, 
                        context.Request.Host));
                }
            }
            
            logger.LogTrace("Found {counter} things", things.Count());
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = Const.ContentType;
            return JsonSerializer.SerializeAsync(context.Response.Body, things, option, context.RequestAborted);
        }
    }
}
