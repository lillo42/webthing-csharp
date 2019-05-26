using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.AspNetCore.Extensions.Middlewares
{
    public abstract class AbstractThingMiddleware
    {
        protected readonly RequestDelegate Next;
        protected ILogger Logger { get; }
        protected IThingType ThingType { get; }
        
        protected AbstractThingMiddleware(RequestDelegate next, ILogger logger, IThingType thingType)
        {
            Next = next;
            Logger = logger;
            ThingType = thingType;
        }

        protected Thing GetThing(HttpContext context)
        {
            RouteData routeData = context.GetRouteData();
            int id = 0;
            
            if(routeData.Values.TryGetValue("thingId", out var data))
            {
                if (data is int number)
                {
                    id = number;
                }
                else
                {
                    int.TryParse(data.ToString(), out id);
                }
            }

            return ThingType[id];
        }
        
        protected static async Task NotFoundAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            await httpContext.Response.WriteAsync("Not found");
        }
        
        protected static async Task BadRequestAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            await httpContext.Response.WriteAsync("Bad request");
        }

        protected static async Task CreatedAsync(HttpContext httpContext, JObject json) 
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.Created;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json.ToString());
        }
        
        protected static Task OkAsync(HttpContext httpContext, JObject json) 
            => OkAsync(httpContext, json.ToString());

        protected static async Task OkAsync(HttpContext httpContext, string json)
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.OK;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json);
        }
    }
}
