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
            return await convert.DeserializeAsync<T>(context.Request.BodyReader,
                context.RequestServices.GetService<IJsonSerializerSettings>(),
                context.RequestAborted);
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
            if (context.GetRouteData().Values.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return default;
        }
    }
}
