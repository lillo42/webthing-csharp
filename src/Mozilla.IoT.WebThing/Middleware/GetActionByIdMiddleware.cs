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
    public class GetActionByIdMiddleware : AbstractThingMiddleware
    {
        public GetActionByIdMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things) 
            : base(next, loggerFactory.CreateLogger<GetActionByIdMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (thing != null)
            {
                string name = httpContext.GetValueFromRoute<string>("actionName");
                if (thing.Actions.Contains(name))
                {
                    string id = httpContext.GetValueFromRoute<string>("actionId");
                    Action action = thing.Actions[name].FirstOrDefault(x => x.Id == id);
                    if (action != null)
                    {
                        var description = httpContext.RequestServices.GetService<IDescription<Action>>();
                        await httpContext.WriteBodyAsync(HttpStatusCode.OK,
                            new Dictionary<string, object> {[name] = description.CreateDescription(action)});
                        return;
                    }
                }
            }

            httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
        }
    }
}
