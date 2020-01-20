using System;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Http
{
    internal static class HttpRequestExtensions
    {
        public static T GetRouteData<T>(this HttpContext request, string key)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var routeData = request.GetRouteData();

            if (routeData.Values.TryGetValue(key, out var result))
            {
                return (T)result;
            }

            return default;
        }
    }
}
