using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetPropertyThingMiddleware : AbstractThingMiddleware
    {
        public GetPropertyThingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things)
            : base(next, loggerFactory.CreateLogger<GetPropertyThingMiddleware>(), things)
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

            string propertyName = httpContext.GetValueFromRoute<string>("propertyName");
            
            Property property = thing.Properties.FirstOrDefault(x => x.Name == propertyName);
            
            if (property == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK,
                    new Dictionary<string, object>
                    {
                        [propertyName] =  property.Value
                    });
        }
    }
}
