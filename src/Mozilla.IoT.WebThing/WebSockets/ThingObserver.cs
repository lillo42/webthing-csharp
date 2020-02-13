using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class ThingObserver
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
        
        public HashSet<string> EventsBind { get; } = new HashSet<string>();
        
        public async void OnEvenAdded(object sender, Event @event)
        {
            _logger.LogInformation("Event add received, going to notify Web Socket");
            var sent = JsonSerializer.SerializeToUtf8Bytes(new WebSocketResponse("event", @event),
                _options);
            
            await _socket.SendAsync(sent, WebSocketMessageType.Text, true, _cancellation)
                .ConfigureAwait(false);
        }

        public async void OnPropertyChanged(object sender, PropertyChangedEventArgs property)
        {
            _logger.LogInformation("Event add received, going to notify Web Socket");
            var sent = JsonSerializer.SerializeToUtf8Bytes(new WebSocketResponse("propertyStatus", 
                    _thing.ThingContext.Properties.GetProperties(property.PropertyName)),
                _options);
            
            await _socket.SendAsync(sent, WebSocketMessageType.Text, true, _cancellation)
                .ConfigureAwait(false);
        }

        public async void OnActionChange(object sender, ActionInfo action)
        {
            await _socket.SendAsync(
                    JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object>
                    {
                        ["messageType"] = "actionStatus",
                        [action.GetActionName()] = action
                    }, _options),
                    WebSocketMessageType.Text, true, _cancellation)
                .ConfigureAwait(false);
        }
    }
}
