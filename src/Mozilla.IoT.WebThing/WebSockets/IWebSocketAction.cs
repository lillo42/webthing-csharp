using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public interface IWebSocketAction
    {
        string Action { get; }

        Task ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, JsonElement data,
            JsonSerializerOptions options, IServiceProvider provider, CancellationToken cancellationToken);
    }
}
