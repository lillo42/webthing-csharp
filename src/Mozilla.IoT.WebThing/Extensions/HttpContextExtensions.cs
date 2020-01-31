using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Mozilla.IoT.WebThing;

namespace Microsoft.AspNetCore.Http
{
    internal static class HttpContextExtensions
    {
        [return: MaybeNull]
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

        public static async Task<T> FromBodyAsync<T>(this HttpContext context, JsonSerializerOptions options,
            CancellationToken cancellationToken = default)
        {
            var reader = context.Request.BodyReader;
            T result = default;
            while (!cancellationToken.IsCancellationRequested)
            {
                var readResult = await reader.ReadAsync(cancellationToken)
                    .ConfigureAwait(false);

                var buffer = readResult.Buffer;
                var position = buffer.PositionOf((byte)'}');
                if (position != null)
                {
                    if (buffer.IsSingleSegment)
                    {
                        result = JsonSerializer.Deserialize<T>(buffer.FirstSpan, options);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (readResult.IsCompleted) break;
            }

            return result;
        }

        public static async ValueTask WriteBodyAsync<T>(this HttpContext context, 
            HttpStatusCode statusCode, T value, 
            JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = Const.ContentType;
            context.Response.Headers[HeaderNames.CacheControl] = "no-cache";
            

            if (value != null)
            {
                var buffer = JsonSerializer.SerializeToUtf8Bytes(value, options);

                await context.Response.BodyWriter.WriteAsync(buffer, cancellationToken)
                    .ConfigureAwait(false);

                await context.Response.BodyWriter.FlushAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
