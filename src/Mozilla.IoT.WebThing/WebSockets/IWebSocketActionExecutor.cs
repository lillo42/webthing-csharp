using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public interface IWebSocketActionExecutor
    {
        string Action { get; }

        Task ExecuteAsync(Thing thing, WebSocket webSocket, JObject data, CancellationToken cancellation);
    }
}
