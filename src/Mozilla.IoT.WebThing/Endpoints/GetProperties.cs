using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal class GetProperties
    {
        public static Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var logger = service.GetRequiredService<ILogger<GetProperties>>();
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            
            var name = context.GetRouteData<string>("name");
            
            logger.LogInformation("Requesting Thing. [Name: {name}]", name);
            var thing = things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", name);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Task.CompletedTask;
            }

            var properties = thing.ThingContext.Properties.GetProperties()!;
            logger.LogInformation("Found Thing with {counter} properties. [Name: {name}]", properties.Count, thing.Name);
            
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = Const.ContentType;
            
            return JsonSerializer.SerializeAsync(context.Response.Body, properties, ThingConverter.Options);
        }
    }
}
