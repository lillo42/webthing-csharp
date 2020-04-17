using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Converts;
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

            service.TryAddSingleton(provider =>
            {
                var opt = provider.GetRequiredService<ThingOption>();
                return new JsonSerializerOptions
                {
                    PropertyNamingPolicy = opt.PropertyNamingPolicy,
                    DictionaryKeyPolicy = opt.PropertyNamingPolicy,
                    PropertyNameCaseInsensitive = opt.IgnoreCase,
                    Converters =
                    {
                        new ActionStatusConverter()
                    },
                    IgnoreNullValues = true
                };
            });

            service.AddScoped<ThingObservableResolver>();
            service.AddScoped(provider => provider.GetService<ThingObservableResolver>().Observer);

            service.AddHttpContextAccessor();
            service.TryAddSingleton<IWebSocketAction, RequestAction>();
            service.TryAddSingleton<IWebSocketAction, AddEventSubscription>();
            service.TryAddSingleton<IWebSocketAction, SetThingProperty>();

            service.TryAddTransient<IThingContextFactory, ThingContextFactory>();
            service.TryAddTransient<IThingResponseBuilder, ThingResponseBuilder>();
            service.TryAddTransient<IEventBuilder, EventBuilder>();
            service.TryAddTransient<IActionBuilder, ActionBuilder>();
            service.TryAddTransient<IPropertyBuilder, PropertyBuilder>();
            
            service.TryAddSingleton<IPropertyFactory, PropertyFactory>();
            service.TryAddSingleton<IActionParameterFactory, ActionParameterFactory>();

            service.TryAddScoped<SystemTextJson>();
            service.TryAddScoped<IJsonReader>(provider => provider.GetRequiredService<SystemTextJson>());
            service.TryAddScoped<IJsonWriter>(provider => provider.GetRequiredService<SystemTextJson>());

            service.TryAddScoped<IJsonSchemaValidationFactory, SystemTexJsonSchemaValidationFactory>();
            service.TryAddScoped<IJsonConvertibleFactory, SystemTexJsonConvertibleFactory>();
            service.TryAddScoped<IConvertibleFactory, SystemTextJsonConvertibleFactory>();


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

    internal class ThingObservableResolver
    {
        public ThingObserver? Observer { get; set; }
    }
}
