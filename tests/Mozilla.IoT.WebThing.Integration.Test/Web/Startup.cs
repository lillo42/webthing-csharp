using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Integration.Test.Web.Things;

namespace Mozilla.IoT.WebThing.Integration.Test.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddThings(x => x.ServerName = "test-thing")
                .AddThing<PropertyThing>()
                .AddThing<WebSocketPropertyThing>()
                .AddThing<ActionThing>()
                .AddThing<WebSocketActionThing>()
                .AddThing<EventThing>()
                .AddThing<WebSocketEventThing>();
            
            services.AddWebSockets(opt =>
            {
                opt.KeepAliveInterval = TimeSpan.FromSeconds(2);
            });

            services.AddHostedService<FireEventService>();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            var address = new ServerAddressesFeature();
            address.Addresses.Add("local:9000");
            app.ServerFeatures.Set<IServerAddressesFeature>(address);
            
            app.UseRouting();

            app.UseWebSockets();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapThings();
            });
        }
    }
}
