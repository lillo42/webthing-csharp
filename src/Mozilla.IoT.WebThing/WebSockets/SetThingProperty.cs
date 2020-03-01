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
            foreach (var jsonProperty in data.EnumerateObject())
            {
                if (!thing.ThingContext.Properties.TryGetValue(jsonProperty.Name, out var property))
                {
                    _logger.LogInformation("Property not found. [Thing: {thing}][Property Name: {propertyName}]", thing.Name, jsonProperty.Name);
                    var response = JsonSerializer.SerializeToUtf8Bytes(
                        new WebSocketResponse("error", 
                            new ErrorResponse("404 Not found", "Property not found")), options);

                    socket.SendAsync(response, WebSocketMessageType.Text, true, cancellationToken)
                        .ConfigureAwait(false);
                }

                switch (property.SetValue(jsonProperty.Value))
                {
                    case SetPropertyResult.InvalidValue:
                    {
                        _logger.LogInformation(
                            "Invalid property value. [Thing: {thing}][Property Name: {propertyName}]",
                            thing.Name, jsonProperty.Name);

                        var response = JsonSerializer.SerializeToUtf8Bytes(
                            new WebSocketResponse("error",
                                new ErrorResponse("400 Bad Request", "Invalid property value")), options);

                        socket.SendAsync(response, WebSocketMessageType.Text, true, cancellationToken)
                            .ConfigureAwait(false);
                        break;
                    }
                    case SetPropertyResult.ReadOnly:
                    {
                        _logger.LogInformation("Read-only property. [Thing: {thing}][Property Name: {propertyName}]",
                            thing.Name, jsonProperty.Name);

                        var response = JsonSerializer.SerializeToUtf8Bytes(
                            new WebSocketResponse("error",
                                new ErrorResponse("400 Bad Request", "Read-only property")), options);

                        socket.SendAsync(response, WebSocketMessageType.Text, true, cancellationToken)
                            .ConfigureAwait(false);
                        break;
                    }
                    case SetPropertyResult.Ok:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return Task.CompletedTask;
        }
    }
}
