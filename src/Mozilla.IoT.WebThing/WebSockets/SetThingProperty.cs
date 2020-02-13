using System;
using System.Net.WebSockets;
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
            foreach (var propertyName in thing.ThingContext.PropertiesName)
            {
                if (!data.TryGetProperty(propertyName, out var property))
                {
                    continue;
                }

                var result = thing.ThingContext.Properties.SetProperty(propertyName, data);
                if (result == SetPropertyResult.InvalidValue)
                {
                    _logger.LogInformation("Invalid property value");
                    var response = JsonSerializer.SerializeToUtf8Bytes(new WebSocketResponse("error", new ErrorResponse("400 Bad Request", "Invalid Property")), options);

                    socket.SendAsync(response, WebSocketMessageType.Text, true, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            
            return Task.CompletedTask;
        }
    }
}
