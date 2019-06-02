using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Middleware
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
            => ThingType[context.GetValueFromRoute<int>("thingId")];
    }
}
