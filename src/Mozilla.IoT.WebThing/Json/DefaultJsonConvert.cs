using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json
{
    public class DefaultJsonConvert : IJsonConvert
    {
        private readonly IJsonSerializerSettings _settings;

        public DefaultJsonConvert(IJsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public T Deserialize<T>(ReadOnlySpan<byte> value) 
            => JsonSerializer.Deserialize<T>(value, ToJsonSerializerOptions(_settings));

        public T Deserialize<T>(ReadOnlySpan<byte> value, IJsonSerializerSettings settings) 
            => JsonSerializer.Deserialize<T>(value, ToJsonSerializerOptions(settings));

        public byte[] Serialize<T>(T value) 
            => JsonSerializer.SerializeToUtf8Bytes(value, ToJsonSerializerOptions(_settings));

        public byte[] Serialize(object value)
            => JsonSerializer.SerializeToUtf8Bytes(value, ToJsonSerializerOptions(_settings));

        public byte[] Serialize<T>(T value, IJsonSerializerSettings settings)
            => JsonSerializer.SerializeToUtf8Bytes(value, ToJsonSerializerOptions(settings));

        public byte[] Serialize(object value, IJsonSerializerSettings settings)
            => JsonSerializer.SerializeToUtf8Bytes(value, ToJsonSerializerOptions(settings));


        private static JsonSerializerOptions ToJsonSerializerOptions(IJsonSerializerSettings settings)
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false,
                WriteIndented = settings.WriteIndented,
                IgnoreNullValues = settings.IgnoreNullValues
            };
        }
    }
}
