using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Events;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.WebSockets
{
    internal class ThingObserver
    {
        private readonly ILogger<ThingObserver> _logger;
        private readonly IJsonConvert _convert;
        
        public ThingObserver(IJsonConvert convert, 
            ILogger<ThingObserver> logger)
        {
            _convert = convert ?? throw new ArgumentNullException(nameof(convert));
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
                _logger.LogWarning("The sender is not a Thing, going to skip notify property changed. [Property: {propertyName}]",
                    property.PropertyName);
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

            if (!propertyValue.TryGetValue(out var value))
            {
                _logger.LogInformation(
                    "Property is write only, not going to notify property change via Web Socket. [Property: {propertyName}][Thing: {thingName}]",
                    property.PropertyName, thing.Name);
                return;
            }

            var sent = _convert.Serialize(new WebSocketResponse("propertyStatus",
                new Dictionary<string, object?> {[propertyName] = value}));

            _logger.LogInformation("Going to notify property change via Web Socket. [Property: {propertyName}][Thing: {thingName}]",
                property.PropertyName, thing.Name);

            foreach (var (_, socket) in thing.ThingContext.Sockets.ToArray())
            {
                if (socket.State != WebSocketState.Open || socket.CloseStatus.HasValue)
                {
                    _logger.LogInformation("The Web Socket is not open or was requested to close. [Property: {propertyName}][Thing: {thingName}]",
                        property.PropertyName, thing.Name);
                    return;
                }

                await socket.SendAsync(sent, WebSocketMessageType.Text, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }
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
