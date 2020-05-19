using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Events;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.WebSockets
{
    
    internal class ThingObserver
    {
        private readonly ILogger<ThingObserver> _logger;
        private readonly System.Net.WebSockets.WebSocket _socket;
        private readonly IJsonConvert _convert;
        private readonly CancellationToken _cancellation;
        
        public ThingObserver(
            System.Net.WebSockets.WebSocket socket, 
            IJsonConvert convert,
            CancellationToken cancellation, 
            ILogger<ThingObserver> logger)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _convert = convert ?? throw new ArgumentNullException(nameof(convert));
            _cancellation = cancellation;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IEnumerable<string> EventsBind { get; } = new HashSet<string>();
        
        public async void OnEvenAdded(object? sender, Event @event)
        {
            // if (sender == null)
            // {
            //     return;
            // }
            //
            // _logger.LogInformation("Event add received, going to notify Web Socket");
            // var sent = JsonSerializer.SerializeToUtf8Bytes(new WebSocketResponse("event", 
            //         new Dictionary<string, object?>
            //         {
            //             [sender.ToString()!] = @event
            //         }), _options);
            //
            // await _socket.SendAsync(sent, WebSocketMessageType.Text, true, _cancellation)
            //     .ConfigureAwait(false);
        }

        public async void OnPropertyChanged(object sender, PropertyChangedEventArgs property)
        {
            if (!(sender is Thing thing))
            {
                return;
            }
            
            var (propertyName, propertyValue) = thing.ThingContext
                .Properties.FirstOrDefault(x => x.Value.OriginPropertyName.Equals(property.PropertyName));

            if (string.IsNullOrEmpty(propertyName))
            {
                _logger.LogWarning("Property not found. [Property: {propertyName}][Thing: {thingName}]",
                    property.PropertyName, thing.Name);
                return;
            }
            
            _logger.LogInformation("Property changed, going to notify via Web Socket. [Property: {propertyName}][Thing: {thingName}]",
                property.PropertyName, thing.Name);

            if (!propertyValue.TryGetValue(out var value))
            {
                _logger.LogInformation(
                    "Property is write only, not going to notify via Web Socket. [Property: {propertyName}][Thing: {thingName}]",
                    property.PropertyName, thing.Name);
                return;
            }

            var sent = _convert.Serialize(new WebSocketResponse("propertyStatus",
                new Dictionary<string, object?> {[propertyName] = value}));

            if (_socket.State != WebSocketState.Open || _socket.CloseStatus.HasValue)
            {
                _logger.LogInformation("The Web Socket is not open. [Property: {propertyName}][Thing: {thingName}]",
                    property.PropertyName, thing.Name);
                return;
            }

            await _socket.SendAsync(sent, WebSocketMessageType.Text, true, _cancellation)
                .ConfigureAwait(false);
        }
        
        public async void OnActionChange(object? sender, ThingActionInformation thingAction)
        {
            // if (sender == null)
            // {
            //     return;
            // }
            //
            // _logger.LogInformation("Action Status changed, going to notify via Web Socket. [Action: {propertyName}][Status: {status}]", thingAction.GetActionName(), thingAction.Status);
            // await _socket.SendAsync(
            //         JsonSerializer.SerializeToUtf8Bytes(new WebSocketResponse("actionStatus",new Dictionary<string, object>
            //         {
            //             [thingAction.GetActionName()] = thingAction
            //         }), _options),
            //         WebSocketMessageType.Text, true, _cancellation)
            //     .ConfigureAwait(false);
        }
    }
}
