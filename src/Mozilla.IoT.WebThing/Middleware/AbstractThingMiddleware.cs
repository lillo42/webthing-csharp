using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Collections;

namespace Mozilla.IoT.WebThing.Middleware
{
    public abstract class AbstractThingMiddleware
    {
        protected readonly RequestDelegate Next;
        protected ILogger Logger { get; }
        protected IThingReadOnlyCollection Things { get; }
        
        protected AbstractThingMiddleware(RequestDelegate next, ILogger logger, IThingReadOnlyCollection things)
        {
            Next = next;
            Logger = logger;
            Things = things;
        }

        protected Thing GetThing(HttpContext context) 
            => Things[context.GetValueFromRoute<string>("thingId")];
    }
}
