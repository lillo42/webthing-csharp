using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Json;

namespace Microsoft.AspNetCore.Http
{
    [ExcludeFromCodeCoverage]
    internal static class HttpContextExtensions
    {
        public static async Task<T> ReadBodyAsync<T>(this HttpContext context)
        {
            var convert =  context.RequestServices.GetService<IJsonConvert>();

            var buffer = new Memory<byte>(new byte[context.Request.Body.Length]);
            await context.Request.Body.ReadAsync(buffer);
            return convert.Deserialize<T>(buffer.Span);
        }

        public static async Task WriteBodyAsync<T>(this HttpContext context, HttpStatusCode statusCode, T value)
        {
            var settings = context.RequestServices.GetService<IJsonSerializerSettings>();
            var convert =  context.RequestServices.GetService<IJsonConvert>();

            byte[] json = convert.Serialize(value, settings);

            context.Response.StatusCode = (int) statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.Body.WriteAsync(new ReadOnlyMemory<byte>(json));
        }

        public static T GetValueFromRoute<T>(this HttpContext context, string key)
        {
            if (context.GetRouteData().Values.TryGetValue(key, out object value))
            {
                return (T)value;
            }

            return default;
        }
    }
}
