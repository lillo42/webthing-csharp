using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Descriptor;
using Mozilla.IoT.WebThing.Json;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing.Notify
{
    internal sealed class NotifySubscribesOnEventAdded
    {
        private readonly Thing _thing;
        private readonly IDescriptor<Event> _descriptor;
        private readonly IJsonConvert _jsonConvert;
        private readonly IJsonSerializerSettings _jsonSettings;

        public NotifySubscribesOnEventAdded(Thing thing, IDescriptor<Event> descriptor,
            IJsonConvert jsonConvert,
            IJsonSerializerSettings jsonSettings)
        {
            _thing = thing;
            _descriptor = descriptor;
            _jsonConvert = jsonConvert;
            _jsonSettings = jsonSettings;
        }

        public async void Notify(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Add 
                && eventArgs.NewItems[0] is Event @event)
            {
                @event.Thing = _thing;
                @event.Metadata = _descriptor.CreateDescription(@event);

                IEnumerable<WebSocket> webSockets = _thing.AvailableEvent.ContainsKey(@event.Name)
                    ? _thing.AvailableEvent[@event.Name].Subscribers
                    : null;
                
                if (webSockets != null && webSockets.Any())
                {
                    var message = new Dictionary<string, object>
                    {
                        [MESSAGE_TYPE] = MessageType.Event.ToString().ToLower(), [DATA] = @event.Metadata
                    };

                    await NotifySubscribersAsync(webSockets, message, CancellationToken.None);
                }
            }
        }

        private async Task NotifySubscribersAsync(IEnumerable<WebSocket> sockets, IDictionary<string, object> message, CancellationToken cancellation)
        {
            var json = _jsonConvert.Serialize(message, _jsonSettings);

            var buffer = new ArraySegment<byte>(json);
            foreach (var socket in sockets)
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation)
                    .ConfigureAwait(false);
            }
        }
    }
}
