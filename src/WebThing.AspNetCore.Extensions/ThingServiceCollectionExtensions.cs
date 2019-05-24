using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.WebSockets;

namespace Microsoft.Extensions.DependencyInjection
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
        
        
        public static void AddThing(this IServiceCollection services, 
            Action<WebSocketOptions> webSocketConfigure,
            Action<CorsOptions> corSetupAction)
        {
            services.AddWebSockets(webSocketConfigure);
            services.AddCors(corSetupAction);
        }
    }
}
