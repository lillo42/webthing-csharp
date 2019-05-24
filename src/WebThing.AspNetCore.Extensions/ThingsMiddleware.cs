using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebThing.AspNetCore.Extensions
{
    public class ThingsMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly IThingType _things;

        public ThingsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory,  IThingType things)
        {
            _logger = loggerFactory.CreateLogger<ThingsMiddleware>();
            _next = next;
            _things = things;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Request.Query
        }

    }
}
