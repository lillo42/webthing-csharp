using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Http
{
    [ExcludeFromCodeCoverage]
    internal static class HttpContextExtensions
    {
        public static T GetValueFromRoute<T>(this HttpContext context, string key)
        {
            if (context.GetRouteData().Values.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return default;
        }
    }
}
