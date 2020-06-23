using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mozilla.IoT.WebThing.Extensions;
using TestThing.Converters;
using TestThing.Things;

namespace TestThing
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddThings(opt =>
                {
                    opt.AddConverters(new DateTimeConverter())
                        .AddConverters(new NullableDateTimeConverter());
                })
                .AddThing<LampThing>();

            services.AddWebSockets(_ => { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.Use(async (context, next) =>
            {
                var url = context.Request.Path.Value;

                // Rewrite to index
                if (url == "/")
                {
                    // rewrite and continue processing
                    context.Request.Path = "/things";
                }
                await next();
            });

            app.UseRouting();
            
            app.UseWebSockets();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapThings();
            });
        }
    }
}
