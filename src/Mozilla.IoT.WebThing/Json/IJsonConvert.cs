using System;

namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonConvert
    {
        T Deserialize<T>(ReadOnlySpan<byte> value);
        T Deserialize<T>(ReadOnlySpan<byte> value, IJsonSerializerSettings settings);
        
        byte[] Serialize<T>(T value);
        byte[] Serialize(object value);
        byte[] Serialize<T>(T value, IJsonSerializerSettings settings);
        byte[] Serialize(object value, IJsonSerializerSettings settings);
    }
}
