using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.WebSockets
{
    /// <summary>
    /// Add event subscription. 
    /// </summary>
    public class AddEventSubscription : IWebSocketAction
    {
        private static readonly  ArraySegment<byte> s_errorActionNotFound = new ArraySegment<byte>(Encoding.UTF8.GetBytes(@"{""messageType"": ""error"",""data"": {""status"": ""404 Not Found"",""message"": ""Event not found""}}"));

        
        /// <inheritdoc/>
        public string Action => "addEventSubscription";
        
        /// <inheritdoc/>
        public async ValueTask ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, object data,
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            var logger = provider.GetRequiredService<ILogger<AddEventSubscription>>();

            var convert = provider.GetRequiredService<IJsonConvert>();
            foreach (var (eventName, _) in convert.ToEnumerable(data))
            {
                if (thing.ThingContext.Events.TryGetValue(eventName, out _))
                {
                    logger.LogInformation("Going to add subscribes socket to event.  [Thing: {thing}][Event: {eventName}]",
                        thing.Name, eventName);
                    
                    thing.ThingContext.EventsSubscribes[eventName]
                        .TryAdd(
                            thing.ThingContext.Sockets.First(x => x.Value == socket).Key, 
                            socket);
                }
                else
                {
                    logger.LogInformation("Event not found. [Thing: {thing}][Event: {eventName}]", thing.Name, eventName);
                    await socket.SendAsync(s_errorActionNotFound, WebSocketMessageType.Text, true, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
