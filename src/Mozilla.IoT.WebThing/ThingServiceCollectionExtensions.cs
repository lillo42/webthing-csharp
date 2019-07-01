using System;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing.Background;
using Mozilla.IoT.WebThing.WebSockets;
using Newtonsoft.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ThingServiceCollectionExtensions
    {
        public static void AddThing(this IServiceCollection services)
            => AddThing(services, option => { });

        public static void AddThing(this IServiceCollection services, JsonSerializerSettings settings)
            => AddThing(services, null, options => { });

        public static void AddThing(this IServiceCollection services, Action<WebSocketOptions> webSocketConfigure)
            => AddThing(services, null, options => { });

        public static void AddThing(this IServiceCollection services, JsonSerializerSettings settings,
            Action<WebSocketOptions> webSocketConfigure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (webSocketConfigure == null)
            {
                throw new ArgumentNullException(nameof(webSocketConfigure));
            }

            services.AddRouting();
            services.AddWebSockets(webSocketConfigure);
            services.AddCors();

            if (settings != null)
            {
                services.TryAddSingleton(settings);
            }
            else
            {
                services.TryAddSingleton(service => new JsonSerializerSettings {Formatting = Formatting.None});
            }

            services.AddHostedService<ActionExecutorHostedService>();

            var block = new BufferBlock<Mozilla.IoT.WebThing.Action>();
            services.AddSingleton<ISourceBlock<Mozilla.IoT.WebThing.Action>>(block);
            services.AddSingleton<ITargetBlock<Mozilla.IoT.WebThing.Action>>(block);

            services.AddTransient<WebSocketProcessor>();

            services.TryAddEnumerable(ServiceDescriptor
                .Transient<IWebSocketActionExecutor, AddEventSubscriptionActionExecutor>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IWebSocketActionExecutor, RequestActionExecutor>());
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IWebSocketActionExecutor, SetPropertyActionExecutor>());
        }
    }
}
