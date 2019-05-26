using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;

namespace Mozilla.IoT.WebThing.AspNetCore.Extensions
{
    public static class ThingServiceCollectionExtensions
    {
        public static void AddThing(this IServiceCollection services) 
            => AddThing(services, wso =>{});

        public static void AddThing(this IServiceCollection services, Action<WebSocketOptions> webSocketConfigure)
        {
            services.AddWebSockets(webSocketConfigure);
            services.AddCors();
        }
    }
}
