using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetActionByIdMiddleware : AbstractThingMiddleware
    {
        public GetActionByIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetActionByIdMiddleware>(), thingType)
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

            string name = httpContext.GetValueFromRoute<string>("actionName");
            string id = httpContext.GetValueFromRoute<string>("actionId");

            Action action = thing.GetAction(name, id);

            if (action == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, new JObject(new JProperty(name, action.AsActionDescription())))
                .ConfigureAwait(false);
        }
    }
}
