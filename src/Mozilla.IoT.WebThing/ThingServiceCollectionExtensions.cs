using System;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing;
using Mozilla.IoT.WebThing.Activator;
using Mozilla.IoT.WebThing.Background;
using Mozilla.IoT.WebThing.Builder;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Descriptor;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.WebSockets;
using Action = Mozilla.IoT.WebThing.Action;
using JsonSerializer = Mozilla.IoT.WebThing.Json.JsonSerializer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ThingServiceCollectionExtensions
    {
        public static void AddThing(this IServiceCollection services)
        {
            AddThing(services, option => { });
        }

        public static void AddThing(this IServiceCollection services, Action<ThingBindingOption> thingOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            if (thingOptions == null)
            {
                throw new ArgumentNullException(nameof(thingOptions));
            }

            var option = new ThingBindingOption();

            thingOptions(option);

            services.AddSingleton(option);
            
            services.AddRouting();
            services.AddWebSockets(options => { });
            services.AddCors();

            services.AddSingleton<ServiceRouteBuilder>();
            
            services.TryAddSingleton(typeof(ThingMarkService));
            services.TryAddSingleton<IThingActivator, ThingActivator>();
            services.TryAddSingleton<IActionActivator, ActionActivator>();

            services.TryAddSingleton<IJsonSerializerSettings>(service => new JsonSerializerSettings(
                new JsonSerializerOptions
                {
                    WriteIndented = false,
                    IgnoreNullValues = true,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                }));

            services.TryAddSingleton<IJsonSerializer, JsonSerializer>();
            services.TryAddSingleton<IJsonSchemaValidator, JsonSchemaValidator>();
            services.TryAddSingleton<IJsonValue, JsonValue>();
            
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddScoped<IHttpRouteValue, HttpRouteValue>();
            services.TryAddScoped<IHttpBodyWriter, HttpPipeWriter>();
            services.TryAddScoped<IHttpBodyReader, HttpPipeReader>();
            services.TryAddSingleton<IWsUrlBuilder, WsUrlBuilder>();

            services.TryAddSingleton<IDescriptor<Action>, ActionDescriptor>();
            services.TryAddSingleton<IDescriptor<Event>, EventDescriptor>();
            services.TryAddSingleton<IDescriptor<Property>, PropertyDescriptor>();
            services.TryAddSingleton<IDescriptor<Thing>, ThingDescriptor>();
            services.TryAddTransient(typeof(IObservableCollection<>), typeof(DefaultObservableCollection<>));

            services.AddHostedService<ActionExecutorHostedService>();

            var block = new BufferBlock<Action>();
            services.AddSingleton<ISourceBlock<Action>>(block);
            services.AddSingleton<ITargetBlock<Action>>(block);

            services.AddTransient<WebSocketProcessor>();

            services.TryAddEnumerable(ServiceDescriptor.Transient<IWebSocketAction, AddEventSubscription>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IWebSocketAction, RequestAction>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IWebSocketAction, SetThingProperty>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IWebSocketAction, GetThingProperty>());
        }
    }
}
