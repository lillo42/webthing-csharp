using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Http
{
    internal static class HttpRequestExtensions
    {
        public static T GetRouteData<T>(this HttpContext request, string key)
        {
            var routeData = request.GetRouteData();

            if (routeData.Values.TryGetValue(key, out var result))
            {
                return (T)result;
            }

            return default;
        }
    }
}
