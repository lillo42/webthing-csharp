using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
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
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                var convert =  context.RequestServices.GetService<IJsonConvert>();
                return convert.Deserialize<T>(await reader.ReadToEndAsync()
                    .ConfigureAwait(false));
            }
        }

        public static async Task WriteBodyAsync<T>(this HttpContext context, HttpStatusCode statusCode, T value)
        {
            var settings = context.RequestServices.GetService<IJsonSerializerSettings>();
            var convert =  context.RequestServices.GetService<IJsonConvert>();

            string json = convert.Serialize(value, settings);

            context.Response.StatusCode = (int) statusCode;
            context.Response.ContentType = "application/json";
            
            await context.Response.WriteAsync(json)
                .ConfigureAwait(false);
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
