using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class AddEventSubscription : IWebSocketAction
    {
        public string Action => "addEventSubscription";

        public Task ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, JsonElement data, JsonSerializerOptions options,
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            foreach (var (@event, collection) in thing.ThingContext.Events)
            {
                if (data.TryGetProperty(@event, out var value))
                {
                    collection.Add += (_, eventData) =>
                    {
                        var sent = JsonSerializer.SerializeToUtf8Bytes(new NotifyEvent(eventData), options);
                        socket.SendAsync(sent, WebSocketMessageType.Text, true, cancellationToken)
                            .ConfigureAwait(false);
                    };
                }
            }
            
            return Task.CompletedTask;
        }
    }

    public class NotifyEvent
    {
        public NotifyEvent(object data)
        {
            Data = data;
        }

        public string MessageType => "event";
        public object Data { get; }
    }
}
