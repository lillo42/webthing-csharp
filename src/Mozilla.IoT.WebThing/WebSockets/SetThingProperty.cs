using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class SetThingProperty : IWebSocketAction
    {
        private readonly ILogger<SetThingProperty> _logger;

        public SetThingProperty(ILogger<SetThingProperty> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Action => "setProperty";

        public Task ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, JsonElement data, JsonSerializerOptions options,
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
