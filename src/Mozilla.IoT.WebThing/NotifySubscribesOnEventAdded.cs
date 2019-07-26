using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.Json;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing
{
    internal sealed class NotifySubscribesOnEventAdded
    {
        private readonly Thing _thing;
        private readonly IDescription<Event> _description;
        private readonly IJsonConvert _jsonConvert;
        private readonly IJsonSerializerSettings _jsonSettings;

        public NotifySubscribesOnEventAdded(Thing thing, 
            IDescription<Event> description, 
            IJsonConvert jsonConvert,
            IJsonSerializerSettings jsonSettings)
        {
            _thing = thing;
            _description = description;
            _jsonConvert = jsonConvert;
            _jsonSettings = jsonSettings;
        }

        public async void Notify(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Add && eventArgs.NewItems[0] is Event @event)
            {
                @event.Thing = _thing;
                @event.Metadata = _description.CreateDescription(@event);

                if (_thing.EventSubscribers.IsEmpty)
                {
                    var message = new Dictionary<string, object>
                    {
                        [MESSAGE_TYPE] = MessageType.Event, [DATA] = @event.Metadata
                    };

                    await NotifySubscribersAsync(message, CancellationToken.None);
                }
            }
        }

        private async Task NotifySubscribersAsync(IDictionary<string, object> message, CancellationToken cancellation)
        {
            byte[] json = _jsonConvert.Serialize(message, _jsonSettings);

            var buffer = new ArraySegment<byte>(json);
            foreach (WebSocket socket in _thing.EventSubscribers.Values)
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation)
                    .ConfigureAwait(false);
            }
        }
    }
}
