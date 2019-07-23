using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetActionMiddleware : AbstractThingMiddleware
    {
        public GetActionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things)
            : base(next, loggerFactory.CreateLogger<GetActionMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (thing != null)
            {
                string name = httpContext.GetValueFromRoute<string>("actionName");

                if (thing.Actions.ContainsKey(name))
                {
                    var description = httpContext.RequestServices.GetService<IDescription<Action>>();
                    var result = thing.Actions[name]
                        .ToDictionary(x => x.Name,
                            x => description.CreateDescription(x));

                    await httpContext.WriteBodyAsync(HttpStatusCode.OK, result);
                }
            }

            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
