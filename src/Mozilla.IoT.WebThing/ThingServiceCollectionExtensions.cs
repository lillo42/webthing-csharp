using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ThingServiceCollectionExtensions
    {
        public static void AddThing(this IServiceCollection services) 
            => AddThing(services, wso =>{});

        public static void AddThing(this IServiceCollection services, Action<WebSocketOptions> webSocketConfigure)
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
        }
    }
}
