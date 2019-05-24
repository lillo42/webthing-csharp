using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace WebThing.AspNetCore.Extensions.Middlewares
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
                
                (description["links"] as JArray).Add(link);
                
                array.Add(description);
            }

            await OkAsync(httpContext, array.ToString());
        }

        private static string GetActionName(HttpContext httpContext)
        {
            if (httpContext.GetRouteData().Values.TryGetValue("actionName", out object data))
            {
                return data.ToString();
            }
            
            return null;
        }
    }
}
