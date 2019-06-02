using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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
            
            var array = new JArray();
            
            foreach (Thing thing in ThingType.Things)
            {
                JObject description = thing.AsThingDescription();
                var link = new JObject(
                    new JProperty("rel", "alternate"),
                    new JProperty("href", $"{ws}/{thing.HrefPrefix}"));
                
                ((JArray)description["links"]).Add(link);
                
                array.Add(description);
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, array.ToString())
                .ConfigureAwait(false);
        }
    }
}
