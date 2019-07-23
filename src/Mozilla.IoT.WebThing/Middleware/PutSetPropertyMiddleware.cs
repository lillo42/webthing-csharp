using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class PutSetPropertyMiddleware : AbstractThingMiddleware
    {
        public PutSetPropertyMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things) 
            : base(next, loggerFactory.CreateLogger<PostActionMiddleware>(), things)
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

            Property property = thing.Properties.FirstOrDefault(x => x.Name == propertyName); 
            if (property == null || !json.ContainsKey(propertyName))
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            var schemaValidator = httpContext.RequestServices.GetService<IJsonSchemaValidator>();
            
            thing.SetProperty(property, json[propertyName], schemaValidator);

            await httpContext.WriteBodyAsync(HttpStatusCode.Created, 
                new Dictionary<string, object>
                {
                    [propertyName] = property.Value
                });
        }
    }
}
