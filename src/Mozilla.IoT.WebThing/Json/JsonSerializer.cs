using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json
{
    public class JsonSerializer : IJsonSerializer
    {
        private readonly IJsonSerializerSettings _settings;

        public JsonSerializer(IJsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public T Deserialize<T>(ReadOnlySpan<byte> value)
            => System.Text.Json.JsonSerializer.Deserialize<T>(value, ToJsonSerializerOptions(_settings));
        
        public byte[] Serialize<T>(T value) 
            => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value, ToJsonSerializerOptions(_settings));
        
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
