using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Properties;

namespace Mozilla.IoT.WebThing.WebSockets
{
    /// <summary>
    /// Set property value action.
    /// </summary>
    public class SetThingProperty : IWebSocketAction
    {
        private readonly ILogger<SetThingProperty> _logger;

        /// <summary>
        /// Initialize a new instance of <see cref="RequestAction"/>.
        /// </summary>
        /// <param name="logger"></param>
        public SetThingProperty(ILogger<SetThingProperty> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The Action name. This value should be unique.
        /// </summary>
        public string Action => "setProperty";

        
        /// <summary>
        /// Execute this action when web socket request action where action name match with <see cref="Action"/>
        /// </summary>
        /// <param name="socket">The <see cref="WebSocket"/> origin of this action.</param>
        /// <param name="thing">The <see cref="Thing"/> associated with action.</param>
        /// <param name="data">The <see cref="JsonElement"/> request with this action.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> for this action. Every request is generate new scope.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        public ValueTask ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, JsonElement data, 
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            var option = provider.GetRequiredService<JsonSerializerOptions>();

            foreach (var jsonProperty in data.EnumerateObject())
            {
                if (!thing.ThingContext.Properties.TryGetValue(jsonProperty.Name, out var property))
                {
                    _logger.LogInformation("Property not found. [Thing: {thing}][Property Name: {propertyName}]", thing.Name, jsonProperty.Name);
                    var response = JsonSerializer.SerializeToUtf8Bytes(
                        new WebSocketResponse("error", 
                            new ErrorResponse("404 Not found", "Property not found")), option);

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
                                new ErrorResponse("400 Bad Request", "Invalid property value")), option);

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
                                new ErrorResponse("400 Bad Request", "Read-only property")), option);

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

            return new ValueTask();
        }
    }
}
