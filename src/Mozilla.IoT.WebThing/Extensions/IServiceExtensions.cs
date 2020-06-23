using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.WebSockets;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class IServiceExtensions
    {
        /// <summary>
        /// Add thing/.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="service"/> if null this will throw <see cref="ArgumentNullException"/>.</exception>
        public static IThingCollectionBuilder AddThings(this IServiceCollection service, Action<ThingOption>? options = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var thingOption = new ThingOption();
            options?.Invoke(thingOption);

            service.AddSingleton(thingOption);
            
            service.AddSingleton<IWebSocketAction, RequestAction>();
            service.AddSingleton<IWebSocketAction, AddEventSubscription>();
            service.AddSingleton<IWebSocketAction, SetThingPropertyWebSocketAction>();

            service.TryAddTransient<IThingContextFactory, ThingContextFactory>();
            service.TryAddTransient<IThingResponseBuilder, ThingResponseBuilder>();
            service.TryAddTransient<IEventBuilder, EventBuilder>();
            service.TryAddTransient<IActionBuilder, ActionBuilder>();
            service.TryAddTransient<IPropertyBuilder, PropertyBuilder>();
            
            service.TryAddSingleton<IPropertyFactory, PropertyFactory>();

            service.TryAddSingleton<SystemTextJson>();
            service.TryAddSingleton<IJsonConvert>(provider => provider.GetRequiredService<SystemTextJson>());
            service.TryAddSingleton<ThingObserver>();

            service.TryAddSingleton<IJsonSchemaValidationFactory, SystemTexJsonSchemaValidationFactory>();
            service.TryAddSingleton<IJsonConvertibleFactory, SystemTexJsonConvertibleFactory>();
            service.TryAddTransient<IConvertibleFactory, ConvertibleFactory>();

            var builder = new ThingCollectionBuilder(service);
            return builder;
        }
    }
}
