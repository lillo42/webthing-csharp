using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Mozilla.IoT.WebThing
{
    internal class HttpRouteValue : IHttpRouteValue
    {
        private readonly RouteData _routeData;

        public HttpRouteValue(IHttpContextAccessor httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            _routeData = httpContext.HttpContext.GetRouteData();
        }

        public T GetValue<T>(string key)
        {
            if (_routeData.Values.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return default;
        }
    }
}
