using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetThingsMiddleware : AbstractThingMiddleware
    {
        public GetThingsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things) 
            : base(next, loggerFactory.CreateLogger<GetThingsMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string ws = string.Empty;
            
            LinkedList<IDictionary<string, object>> array = new LinkedList<IDictionary<string, object>>();
            var descriptor = httpContext.RequestServices.GetService<IDescription<Thing>>();
            
            foreach (Thing thing in Things)
            {
                IDictionary<string, object> description = descriptor.CreateDescription(thing);

                var link = new Dictionary<string, object>
                {
                    ["rel"] = "alternate",
                    ["href"] = $"{ws}/{thing.HrefPrefix}" 
                };
                
                ((ICollection<IDictionary<string, object>>)description["links"]).Add(link);
                array.AddLast(description);
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, array);
        }
    }
}
