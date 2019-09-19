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

        public NotifySubscribesOnPropertyChanged(IJsonSerializer jsonSerializer)
        {
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
        
        private async Task NotifySubscribersAsync(ICollection<WebSocket> subscribers, IDictionary<string, object> message, CancellationToken cancellation)
        {
            var tasks = new List<Task>(subscribers.Count);
            var buffer = new ArraySegment<byte>(_jsonSerializer.Serialize(message));
            foreach (var socket in subscribers)
            {
                tasks.Add(socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation));
            }

            await Task.WhenAll(tasks);
        }
    }
}
