using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing
{
    public interface IActionActivator
    {
        ValueTask<Action> CreateAsync(Thing thing, string name, IDictionary<string, object> input, CancellationToken cancellation);
    }
}
