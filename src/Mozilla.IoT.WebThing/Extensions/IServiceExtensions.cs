using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.WebSockets;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceExtensions
    {
        public static IThingCollectionBuilder AddThings(this IServiceCollection service, Action<ThingOption>? options = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var thingOption = new ThingOption();
            options?.Invoke(thingOption);

            service.AddSingleton(thingOption);

            service.TryAddSingleton(provider =>
            {
                var opt = provider.GetRequiredService<ThingOption>();
                return new JsonSerializerOptions
                {
                    PropertyNamingPolicy = opt.PropertyNamingPolicy,
                    PropertyNameCaseInsensitive = opt.IgnoreCase
                };
            });
            
            service.AddSingleton<IWebSocketAction, RequestAction>();
            service.AddSingleton<IWebSocketAction, AddEventSubscription>();
            
            service.AddSingleton(provider =>
            {
                var opt = provider.GetRequiredService<ThingOption>();
                var actions = provider.GetRequiredService<IEnumerable<IWebSocketAction>>();

                return actions.ToDictionary(
                    x => x.Action,
                    x => x,
                    opt.IgnoreCase ? StringComparer.InvariantCultureIgnoreCase : null);
            });
            
            var builder = new ThingCollectionBuilder(service);
            return builder;
        }
    }
}
