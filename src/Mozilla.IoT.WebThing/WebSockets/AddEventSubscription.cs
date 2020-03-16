using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.WebSockets
{
    /// <summary>
    /// Add event subscription. 
    /// </summary>
    public class AddEventSubscription : IWebSocketAction
    {
        /// <summary>
        /// The Action name. This value should be unique.
        /// </summary>
        public string Action => "addEventSubscription";
        
        /// <summary>
        /// Execute this action when web socket request action where action name match with <see cref="Action"/>
        /// </summary>
        /// <param name="socket">The <see cref="WebSocket"/> origin of this action.</param>
        /// <param name="thing">The <see cref="Thing"/> associated with action.</param>
        /// <param name="data">The <see cref="JsonElement"/> request with this action.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> for this action. Every request is generate new scope.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        public ValueTask ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, JsonElement data,
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            var observer = provider.GetRequiredService<ThingObserver>();
            var logger = provider.GetRequiredService<ILogger<AddEventSubscription>>();

            foreach (var eventName in data.EnumerateObject().TakeWhile(eventName => !cancellationToken.IsCancellationRequested))
            {
                if (thing.ThingContext.Events.TryGetValue(eventName.Name, out var @events))
                {
                    events.Added += observer.OnEvenAdded;
                }
                else
                {
                    logger.LogInformation("{eventName} event not found. [Thing: {thing}]", eventName.Name, thing.Name);
                }
            }

            return new ValueTask();
        }
    }
}
