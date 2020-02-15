using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mozilla.IoT.WebThing.AcceptanceTest.Things;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.AcceptanceTest
{
    public class Startup
    {
        public static Action<ThingOption>? Option { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddThings(Option)
                .AddThing<LampThing>();

            services.AddWebSockets(o => { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseWebSockets();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapThings();
            });
        }
    }
}
