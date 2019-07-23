using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Middleware
{
    public abstract class AbstractThingMiddleware
    {
        protected readonly RequestDelegate Next;
        protected ILogger Logger { get; }
        protected IReadOnlyList<Thing> Things { get; }
        
        protected AbstractThingMiddleware(RequestDelegate next, ILogger logger, IReadOnlyList<Thing> things)
        {
            Next = next;
            Logger = logger;
            Things = things;
        }

        protected Thing GetThing(HttpContext context) 
            => Things[context.GetValueFromRoute<int>("thingId")];
    }
}
