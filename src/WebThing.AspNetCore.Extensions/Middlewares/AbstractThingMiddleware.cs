using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace WebThing.AspNetCore.Extensions.Middlewares
{
    public abstract class AbstractMiddleware
    {
        protected readonly RequestDelegate Next;
        protected ILogger Logger { get; }
        protected IThingType Thing { get; }
        
        protected AbstractMiddleware(RequestDelegate next, ILogger logger, IThingType thing)
        {
            Next = next;
            Logger = logger;
            Thing = thing;
        }

        protected Thing GetThing(HttpContext context)
        {
            RouteData routeData = context.GetRouteData();
            KeyValuePair<string, object> data = routeData.Values.FirstOrDefault();

            int id = 0;

            if (data.Value is int number)
            {
                id = number;
            }
            else
            {
                int.TryParse(data.Value.ToString(), out id);
            }

            return Thing[id];
        }
    }
}
