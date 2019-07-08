using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PutSetPropertyMiddleware : AbstractThingMiddleware
    {
        public PutSetPropertyMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<PostActionMiddleware>(), thingType)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (thing == null)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            string propertyName = httpContext.GetValueFromRoute<string>("propertyName");
            
            var json = await httpContext.ReadBodyAsync<IDictionary<string, object>>();
            
            if (!thing.ContainsProperty(propertyName) || !json.ContainsKey(propertyName))
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            thing.SetProperty(propertyName, json[propertyName]);

            await httpContext.WriteBodyAsync(HttpStatusCode.Created, 
                new Dictionary<string, object>
                {
                    [propertyName] = thing.GetProperty(propertyName)
                });
        }
    }
}
