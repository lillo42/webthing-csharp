using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing
{
    internal interface IHttpBodyReader
    {
        ValueTask<T> ReadAsync<T>(CancellationToken cancellationToken = default);
    }
}
