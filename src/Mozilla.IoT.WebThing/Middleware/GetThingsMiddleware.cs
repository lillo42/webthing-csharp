using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetThingsMiddleware : AbstractThingMiddleware
    {
        public GetThingsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetThingsMiddleware>(), thingType)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string ws = string.Empty;
            
            ICollection<IDictionary<string, object>> array = new LinkedList<IDictionary<string, object>>();
            
            foreach (Thing thing in ThingType.Things)
            {
                IDictionary<string, object> description = thing.AsThingDescription();

                var link = new Dictionary<string, object>
                {
                    ["rel"] = "alternate",
                    ["href"] = $"{ws}/{thing.HrefPrefix}" 
                };
                
                ((ICollection<IDictionary<string, object>>)description["links"]).Add(link);
                array.Add(description);
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, array);
        }
    }
}
