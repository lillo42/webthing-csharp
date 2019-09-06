using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Json;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing.Notify
{
    internal sealed class NotifySubscribesOnPropertyChanged
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IJsonSerializerSettings _jsonSettings;

        public NotifySubscribesOnPropertyChanged(
            IJsonSerializer jsonSerializer,
            IJsonSerializerSettings jsonSettings)
        {
            _jsonSettings = jsonSettings;
            _jsonSerializer = jsonSerializer;
        }


        public async void Notify(object sender, ValueChangedEventArgs eventArgs)
        {
            if (sender is Property property && !property.Thing.Subscribers.IsEmpty)
            {
                var message = new Dictionary<string, object>
                {
                    [MESSAGE_TYPE] = "propertyStatus", 
                    [DATA] = new Dictionary<string, object>
                    {
                        [property.Name] = eventArgs.Value
                    }
                };

                await NotifySubscribersAsync(property.Thing.Subscribers.Values, message, CancellationToken.None);
            }
        }
        
        private async Task NotifySubscribersAsync(IEnumerable<WebSocket> subscribers, IDictionary<string, object> message, CancellationToken cancellation)
        {
            var json = _jsonSerializer.Serialize(message, _jsonSettings);

            var buffer = new ArraySegment<byte>(json);
            foreach (var socket in subscribers)
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation)
                    .ConfigureAwait(false);
            }
        }
    }
}
