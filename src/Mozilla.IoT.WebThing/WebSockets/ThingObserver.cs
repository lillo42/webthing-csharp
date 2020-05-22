using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
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
        private readonly ThingOption _option;
        private readonly ILogger<ThingObserver> _logger;
        private readonly IJsonConvert _convert;
        
        public ThingObserver(IJsonConvert convert,
            ThingOption option,
            ILogger<ThingObserver> logger)
        {
            _convert = convert ?? throw new ArgumentNullException(nameof(convert));
            _option = option ?? throw new ArgumentNullException(nameof(option));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IEnumerable<string> EventsBind { get; } = new HashSet<string>();
        
        public async void OnEvenAdded(object? sender, EventAddedEventArgs args)
        {
            if (!(sender is Thing thing))
            {
                _logger.LogWarning("The sender is not a Thing, not going to notify.");
                return;
            }

            if (thing.ThingContext.EventsSubscribes[args.EventName].IsEmpty)
            {
                _logger.LogTrace("No subscriber for this event.  [Thing: {thingName}][Event: {eventName}]",
                    thing.Name, args.EventName);
                return;
            }
            
            var message = _convert.Serialize(new WebSocketResponse("event",
                new Dictionary<string, object?>
                {
                    [_option.PropertyNamingPolicy.ConvertName(args.EventName)] = args.Event
                }));

            _logger.LogInformation("Going to notify event via Web Socket. [Thing: {thingName}][Event: {eventName}]",
                thing.Name, args.EventName);
            
            foreach (var (_, socket) in thing.ThingContext.EventsSubscribes[args.EventName].ToArray())
            {
                if (socket.State != WebSocketState.Open || socket.CloseStatus.HasValue)
                {
                    _logger.LogInformation("The Web Socket is not open or was requested to close. [Thing: {thingName}][Event: {eventName}]",
                        thing.Name, args.EventName);
                    return;
                }

                await socket.SendAsync(message, WebSocketMessageType.Text, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }
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
                _logger.LogWarning("Property not found. [Thing: {thingName}][Property: {propertyName}]",
                    thing.Name, property.PropertyName);
                return;
            }

            if (!propertyValue.TryGetValue(out var value))
            {
                _logger.LogInformation(
                    "Property is write only, not going to notify property change via Web Socket.[Thing: {thingName}][Property: {propertyName}]",
                    thing.Name, property.PropertyName);
                return;
            }

            var message = _convert.Serialize(new WebSocketResponse("propertyStatus",
                new Dictionary<string, object?> {[propertyName] = value}));

            _logger.LogInformation("Going to notify property change via Web Socket. [Thing: {thingName}][Property: {propertyName}]",
                thing.Name, property.PropertyName);

            foreach (var (_, socket) in thing.ThingContext.Sockets.ToArray())
            {
                if (socket.State != WebSocketState.Open || socket.CloseStatus.HasValue)
                {
                    _logger.LogInformation("The Web Socket is not open or was requested to close. [Property: {propertyName}][Thing: {thingName}]",
                        property.PropertyName, thing.Name);
                    return;
                }

                await socket.SendAsync(message, WebSocketMessageType.Text, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }
        
        public async void OnActionChange(object? sender, ThingActionInformation thingAction)
        {
            if (thingAction.Thing == null)
            {
                _logger.LogWarning("Going to skip notify action changed. [Action: {actionName}]",
                    thingAction.GetActionName());
                return;
            }

            var thing = thingAction.Thing;
            
            _logger.LogInformation("Action Status changed, going to notify via Web Socket. [Thing: {name}][Action: {actionName}][Action Id: {actionId}][Status: {status}]", 
                thing.Name, thingAction.GetActionName(), thingAction.GetId(), thingAction.Status);
            
            var message = _convert.Serialize(new WebSocketResponse("actionStatus",
                new Dictionary<string, object?>
                {
                    [_option.PropertyNamingPolicy.ConvertName(thingAction.GetActionName())] = thingAction
                }));
            
            foreach (var (_, socket) in thing.ThingContext.Sockets.ToArray())
            {
                if (socket.State != WebSocketState.Open || socket.CloseStatus.HasValue)
                {
                    _logger.LogInformation("The Web Socket is not open or was requested to close. [Thing: {name}][Action: {actionName}][Action Id: {actionId}][Status: {status}]", 
                        thing.Name, thingAction.GetActionName(), thingAction.GetId(), thingAction.Status);
                    continue;
                }

                await socket.SendAsync(message, WebSocketMessageType.Text, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }
    }
}
