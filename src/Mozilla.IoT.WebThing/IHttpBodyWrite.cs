using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing
{
    internal interface IHttpBodyWriter
    {
        ValueTask WriteAsync<T>(T value, CancellationToken cancellationToken = default);
    }
}
