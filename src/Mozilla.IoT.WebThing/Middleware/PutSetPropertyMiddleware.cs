using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PutSetPropertyMiddleware : AbstractThingMiddleware
    {
        public PutSetPropertyMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingReadOnlyCollection things) 
            : base(next, loggerFactory.CreateLogger<PostActionMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var thingId = httpContext.GetValueFromRoute<string>("thing");
            var propertyName = httpContext.GetValueFromRoute<string>("propertyName");
            Logger.LogInformation($"Put Property is calling: [[thing: {thingId}][property: {propertyName}]]");
            
            var thing = Things[thingId];

            if (thing == null)
            {
                Logger.LogInformation($"Put Property: Thing not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var json = await httpContext.ReadBodyAsync<IDictionary<string, object>>();
            if (json == null)
            {
                Logger.LogInformation($"Put Property: Body not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }

            if (!json.Keys.Any()) 
            {
                Logger.LogInformation($"Put Property: Body is empty [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }

            Property property = thing.Properties.FirstOrDefault(x => x.Name == propertyName); 
            if (property == null || !json.ContainsKey(propertyName))
            {
                Logger.LogInformation($"Put Property: Property not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            property.Value = json[propertyName];

            await httpContext.WriteBodyAsync(HttpStatusCode.Created, 
                new Dictionary<string, object>
                {
                    [propertyName] = property.Value
                });
        }
    }
}
