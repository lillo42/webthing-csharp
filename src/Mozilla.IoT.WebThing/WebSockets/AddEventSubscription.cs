using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class AddEventSubscription : IWebSocketAction
    {
        public string Action => "addEventSubscription";

        public Task ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, JsonElement data,
            JsonSerializerOptions options,
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            var observer = provider.GetRequiredService<ThingObserver>();
            foreach (var (@event, collection) in thing.ThingContext.Events)
            {
                if (data.TryGetProperty(@event, out _))
                {
                    collection.Added += observer.OnEvenAdded;
                }
            }

            return Task.CompletedTask;
        }
    }
}
