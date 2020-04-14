using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozilla.IoT.WebThing.Json
{
    /// <summary>
    /// Implementation of <see cref="IJsonReader"/> for System.Text.Json
    /// </summary>
    public class SystemTextJson : IJsonReader, IJsonWriter
    {
        private readonly JsonSerializerOptions _options;
        private readonly IHttpContextAccessor _context;

        /// <summary>
        /// Initialize a new instance of <see cref="SystemTextJson"/>.
        /// </summary>
        /// <param name="context">The <see cref="IHttpContextAccessor"/>.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/>.</param>
        public SystemTextJson(IHttpContextAccessor context, JsonSerializerOptions options)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, object?>> GetValuesAsync()
        {
            var context = _context.HttpContext;
            
            var reader = context.Request.BodyReader;
            var cancellationToken = _context.HttpContext.RequestAborted;
            Dictionary<string, object?> result = default!;
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
                        result = JsonSerializer.Deserialize<Dictionary<string, object?>>(buffer.FirstSpan, _options);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (readResult.IsCompleted)
                {
                    break;
                }
            }
            
            return result;
        }

        /// <inheritdoc/>
        public async Task WriteAsync<T>(T value)
        {
            var context = _context.HttpContext;
            var cancellationToken = _context.HttpContext.RequestAborted;
            
            if (value != null)
            {
                var buffer = JsonSerializer.SerializeToUtf8Bytes(value, value.GetType(), _options);

                await context.Response.BodyWriter.WriteAsync(buffer, cancellationToken)
                    .ConfigureAwait(false);

                await context.Response.BodyWriter.FlushAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
