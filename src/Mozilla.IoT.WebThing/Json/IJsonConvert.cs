using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonConvert
    {
        ValueTask<T> DeserializeAsync<T>(PipeReader reader, IJsonSerializerSettings settings, CancellationToken cancellation = default);
        
        T Deserialize<T>(ReadOnlySpan<byte> value);
        T Deserialize<T>(ReadOnlySpan<byte> value, IJsonSerializerSettings settings);
        
        byte[] Serialize<T>(T value);
        byte[] Serialize(object value);
        byte[] Serialize<T>(T value, IJsonSerializerSettings settings);
        byte[] Serialize(object value, IJsonSerializerSettings settings);
    }
}
