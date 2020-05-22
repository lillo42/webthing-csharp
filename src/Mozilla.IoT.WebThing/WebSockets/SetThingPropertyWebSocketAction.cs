using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.WebSockets
{
    /// <summary>
    /// Set property value action.
    /// </summary>
    public class SetThingPropertyWebSocketAction : IWebSocketAction
    {
        private readonly ILogger<SetThingPropertyWebSocketAction> _logger;

        /// <summary>
        /// Initialize a new instance of <see cref="SetThingPropertyWebSocketAction"/>.
        /// </summary>
        /// <param name="logger"></param>
        public SetThingPropertyWebSocketAction(ILogger<SetThingPropertyWebSocketAction> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public string Action => "setProperty";

        /// <inheritdoc/>
        public async ValueTask ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, object data, 
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            var convert = provider.GetRequiredService<IJsonConvert>();
            foreach (var (propertyName, value) in convert.ToEnumerable(data))
            {
                if (!thing.ThingContext.Properties.TryGetValue(propertyName, out var property))
                {
                    _logger.LogInformation("Property not found. [Thing: {thing}][Property Name: {propertyName}]", 
                        thing.Name, propertyName);
                    var response = convert.Serialize(new WebSocketResponse("error",
                        new ErrorResponse("404 Not found", $"{propertyName} property not found")));
            
                    await socket.SendAsync(response, WebSocketMessageType.Text, true, cancellationToken)
                        .ConfigureAwait(false);
                }
            
                switch (property!.TrySetValue(value))
                {
                    case SetPropertyResult.InvalidValue:
                    {
                        _logger.LogInformation(
                            "Invalid property value. [Thing: {thing}][Property Name: {propertyName}]",
                            thing.Name, propertyName);
            
                        var response = convert.Serialize(new WebSocketResponse("error",
                            new ErrorResponse("400 Bad Request", $"Invalid value for {propertyName} property.")));
            
                        await socket.SendAsync(response, WebSocketMessageType.Text, true, cancellationToken)
                            .ConfigureAwait(false);
                        break;
                    }
                    case SetPropertyResult.ReadOnly:
                    {
                        _logger.LogInformation("Read-only property. [Thing: {thing}][Property Name: {propertyName}]",
                            thing.Name, propertyName);
            
                        var response = convert.Serialize(new WebSocketResponse("error",
                            new ErrorResponse("400 Bad Request", $"{propertyName} property is read-only")));
            
                        await socket.SendAsync(response, WebSocketMessageType.Text, true, cancellationToken)
                            .ConfigureAwait(false);
                        break;
                    }
                    case SetPropertyResult.Ok:
                        _logger.LogInformation(
                            "Set property value with success. [Thing: {thing}][Property Name: {propertyName}]",
                            thing.Name, propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
