using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing
{
    internal class HttpPipeReader : IHttpBodyReader
    {
        private readonly IJsonSerializer _serializer;
        private readonly IJsonSerializerSettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpPipeReader(IJsonSerializer serializer,
            IJsonSerializerSettings settings,
            IHttpContextAccessor httpContextAccessor)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async ValueTask<T> ReadAsync<T>(CancellationToken cancellationToken = default)
        {
            var pipe = _httpContextAccessor.HttpContext.Request.BodyReader;
            var buffer = await pipe.ReadAsync(cancellationToken);
            return _serializer.Deserialize<T>(buffer.Buffer.FirstSpan);
        }
    }
}
