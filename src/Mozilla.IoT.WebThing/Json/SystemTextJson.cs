using System;
using System.Collections.Generic;
using System.Text.Json;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Json
{
    /// <summary>
    /// Implementation of <see cref="IJsonConvert"/> for System.Text.Json
    /// </summary>
    public class SystemTextJson : IJsonConvert
    {
        private readonly ThingOption _options;

        /// <summary>
        /// Initialize a new instance of <see cref="SystemTextJson"/>.
        /// </summary>
        /// <param name="options">The <see cref="ThingOption"/>.</param>
        public SystemTextJson(ThingOption options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc/>
        public T Deserialize<T>(ReadOnlySpan<byte> values)
        {
            return JsonSerializer.Deserialize<T>(values, _options.ToJsonSerializerOptions());
        }

        /// <inheritdoc/>
        public byte[] Serialize<T>(T value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, _options.ToJsonSerializerOptions());
        }

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, object>> ToEnumerable(object data)
        {
            if (!(data is JsonElement element))
            {
                yield break;
            }

            foreach (var property in element.EnumerateObject())
            {
                yield return new KeyValuePair<string, object>(property.Name, property.Value);
            }
        }
    }
}
