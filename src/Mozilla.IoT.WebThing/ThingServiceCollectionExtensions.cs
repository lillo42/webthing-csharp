using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Background;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.WebSockets;
using Action = System.Action;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ThingServiceCollectionExtensions
    {
        #region Singles

        public static void AddThing<T>(this IServiceCollection services)
            where T : Thing
        {
            RegisterCommon(services);
            services.AddSingleton(typeof(T));
            services.TryAddSingleton<IReadOnlyList<Thing>>(provider =>
            {
                Thing thing = (Thing)provider.GetService(typeof(T));
                RegisterActions(services, thing);
                return new SingleThingCollection(thing);
            });
        }

        #endregion

        #region Multi
        public static void AddThing(this IServiceCollection services, Action<ThingBindingOption> thingOptions)
        {
            RegisterCommon(services);

            if (thingOptions == null)
            {
                throw new ArgumentNullException(nameof(thingOptions));
            }
            
            var option = new ThingBindingOption();

            thingOptions(option);

            foreach (Type thing in option.ThingsType)
            {
                services.AddSingleton(thing);
            }

            foreach (Thing thing in option.Things)
            {
                services.AddSingleton(thing);
                RegisterActions(services, thing);
            }

            services.TryAddSingleton<IReadOnlyList<Thing>>(provider =>
            {
                var things = new List<Thing>(option.ThingsType.Count + option.Things.Count);
                foreach (Type thingType in option.ThingsType)
                {
                    var thing = (Thing)provider.GetService(thingType);
                    RegisterActions(services, thing);
                    things.Add(thing);
                }

                foreach (Thing thing in option.Things)
                {
                    things.Add(thing);
                }

                if (things.Count == 1 && !option.IsMultiThing)
                {
                    return new SingleThingCollection(things[0]);   
                }
                else
                {
                    return new MultipleThingsCollections(things);    
                }
            });
        }

        #endregion

        private static void RegisterCommon(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddRouting();
            services.AddWebSockets(options => { });
            services.AddCors();

            services.TryAddSingleton<IJsonSerializerSettings>(service => new DefaultJsonSerializerSettings(
                new JsonSerializerOptions
                {
                    WriteIndented = false,
                    IgnoreNullValues = true,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                }));

            services.TryAddSingleton<IJsonConvert, DefaultJsonConvert>();
            services.TryAddSingleton<IJsonSchemaValidator, DefaultJsonSchemaValidator>();

            services.AddHostedService<ActionExecutorHostedService>();

            var block = new BufferBlock<Mozilla.IoT.WebThing.Action>();
            services.AddSingleton<ISourceBlock<Mozilla.IoT.WebThing.Action>>(block);
            services.AddSingleton<ITargetBlock<Mozilla.IoT.WebThing.Action>>(block);

            services.AddTransient<WebSocketProcessor>();

            services.TryAddEnumerable(ServiceDescriptor.Transient<IWebSocketAction, AddEventSubscription>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IWebSocketAction, RequestAction>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IWebSocketAction, SetThingProperty>());
        }

        private static void RegisterActions(IServiceCollection services, Thing thing)
        {
            foreach ((Type type, _) in thing.ActionsTypes.Values)
            {
                services.AddTransient(type);
            }
        }
    }
}
