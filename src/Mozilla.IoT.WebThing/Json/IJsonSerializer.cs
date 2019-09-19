using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(ReadOnlySpan<byte> value);
        byte[] Serialize<T>(T value);
    }
}
