using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Extensions
{
    internal static class WebSocketCollectionExtensions
    {
        public static async Task NotifySubscriberAsync(this IEnumerable<WebSocket> source, IDictionary<string, object> data,
            IServiceProvider serviceProvider, CancellationToken cancellation)
        {
            var settings = serviceProvider.GetService<IJsonSerializerSettings>();
            var convert =  serviceProvider.GetService<IJsonConvert>();
            
            byte[] json = convert.Serialize(data, settings);
            
            var buffer = new ArraySegment<byte>(json);
            foreach (WebSocket socket in source)
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true,cancellation);
            }
        }
    }
}
