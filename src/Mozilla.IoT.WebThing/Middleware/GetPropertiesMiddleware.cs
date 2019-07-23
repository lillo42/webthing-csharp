using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetPropertiesMiddleware : AbstractThingMiddleware
    {
        public GetPropertiesMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things) 
            : base(next, loggerFactory.CreateLogger<GetPropertiesMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (thing == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var result = new Dictionary<string, object>();

            foreach (Property property in thing.Properties)
            {
                result.Add(property.Name, property.Value);
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, result)
                .ConfigureAwait(false);
        }
    }
}
