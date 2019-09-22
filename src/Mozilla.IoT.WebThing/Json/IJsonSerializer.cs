using System;

namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(ReadOnlySpan<byte> value);
        byte[] Serialize<T>(T value);
    }
}
