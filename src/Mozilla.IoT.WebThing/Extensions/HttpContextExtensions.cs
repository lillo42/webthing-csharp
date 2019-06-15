using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.Http
{
    [ExcludeFromCodeCoverage]
    internal static class HttpContextExtensions
    {
        public static async Task<T> ReadBodyAsync<T>(this HttpContext context)
        {
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync()
                    .ConfigureAwait(false));
            }
        }
        
        
        public static async Task WriteBodyAsync<T>(this HttpContext context, HttpStatusCode statusCode, T value)
        {
            JsonSerializerSettings settings = context.RequestServices.GetService<JsonSerializerSettings>();
            
            string json =  value switch
            {
                JToken token => token.ToString(settings.Formatting),
                _ => JsonConvert.SerializeObject(value, settings) 
            };
            
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
