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

namespace Mozilla.IoT.WebThing.WebSockets
{
    
    internal class ThingObserver
    {
        private readonly ILogger<ThingObserver> _logger;
        private readonly Thing _thing;
        private readonly JsonSerializerOptions _options;
        private readonly System.Net.WebSockets.WebSocket _socket;
        private readonly CancellationToken _cancellation;
        
        public ThingObserver(ILogger<ThingObserver> logger, 
            JsonSerializerOptions options, 
            System.Net.WebSockets.WebSocket socket, 
            CancellationToken cancellation, 
            Thing thing)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _cancellation = cancellation;
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
        }
        
        public IEnumerable<string> EventsBind { get; } = new HashSet<string>();
        
        public async void OnEvenAdded(object? sender, Event @event)
        {
            if (sender == null)
            {
                return;
            }
            
            _logger.LogInformation("Event add received, going to notify Web Socket");
            var sent = JsonSerializer.SerializeToUtf8Bytes(new WebSocketResponse("event", 
                    new Dictionary<string, object?>
                    {
                        [sender.ToString()!] = @event
                    }), _options);
            
            await _socket.SendAsync(sent, WebSocketMessageType.Text, true, _cancellation)
                .ConfigureAwait(false);
        }

        public async void OnPropertyChanged(object sender, PropertyChangedEventArgs property)
        {
            var (propertyName, propertyValue) = _thing.ThingContext
                .Properties.First(x => x.Value.OriginPropertyName.Equals(property.PropertyName));

            _logger.LogInformation("Property changed, going to notify via Web Socket. [Property: {propertyName}]",
                property.PropertyName);

            if (!propertyValue.TryGetValue(out var value))
            {
                _logger.LogInformation(
                    "Property is write only, not going to notify via Web Socket. [Property: {propertyName}]",
                    property.PropertyName);
                return;
            }

            var sent = JsonSerializer.SerializeToUtf8Bytes(new WebSocketResponse("propertyStatus",
                new Dictionary<string, object?> {[propertyName] = value}), _options);

            if (_socket.State != WebSocketState.Open || _socket.CloseStatus.HasValue)
            {
                _logger.LogInformation(
                    "The Web Socket is not open. [Property: {propertyName}]",
                    property.PropertyName);
                return;
            }

            await _socket.SendAsync(sent, WebSocketMessageType.Text, true, _cancellation)
                .ConfigureAwait(false);
        }
        
        public async void OnActionChange(object? sender, ThingActionInformation thingAction)
        {
            if (sender == null)
            {
                return;
            }
            
            _logger.LogInformation("Action Status changed, going to notify via Web Socket. [Action: {propertyName}][Status: {status}]", thingAction.GetActionName(), thingAction.Status);
            await _socket.SendAsync(
                    JsonSerializer.SerializeToUtf8Bytes(new WebSocketResponse("actionStatus",new Dictionary<string, object>
                    {
                        [thingAction.GetActionName()] = thingAction
                    }), _options),
                    WebSocketMessageType.Text, true, _cancellation)
                .ConfigureAwait(false);
        }
    }
}
