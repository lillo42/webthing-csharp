using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.Json;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing.Notify
{
    internal sealed class NotifySubscribesOnActionAdded
    {
        private readonly IDescriptor<Action> _descriptor;
        private readonly IJsonConvert _jsonConvert;
        private readonly IJsonSerializerSettings _jsonSettings;

        public NotifySubscribesOnActionAdded(IDescriptor<Action> descriptor, 
            IJsonConvert jsonConvert,
            IJsonSerializerSettings jsonSettings)
        {
            _descriptor = descriptor;
            _jsonConvert = jsonConvert;
            _jsonSettings = jsonSettings;
        }

        public async void Notify(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Add 
                && eventArgs.NewItems[0] is Action action 
                && !action.Thing.Subscribers.IsEmpty)
            {
                var message = new Dictionary<string, object>
                {
                    [MESSAGE_TYPE] = "actionStatus",
                    [DATA] = new Dictionary<string, object>
                    {
                        [action.Name] = _descriptor.CreateDescription(action)
                    }
                };

                await NotifySubscribersAsync(action.Thing.Subscribers.Values, message, CancellationToken.None);
            }
        }

        private async Task NotifySubscribersAsync(IEnumerable<WebSocket> subscribers, IDictionary<string, object> message, CancellationToken cancellation)
        {
            byte[] json = _jsonConvert.Serialize(message, _jsonSettings);

            var buffer = new ArraySegment<byte>(json);
            foreach (WebSocket socket in subscribers)
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation)
                    .ConfigureAwait(false);
            }
        }
    }
}
