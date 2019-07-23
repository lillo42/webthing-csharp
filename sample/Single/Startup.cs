using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing;

namespace Single
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var light = new Thing
            {
                Name = "My Lamp", Type = new[] {"OnOffSwitch", "Light"}, Description = "A web connected lamp"
            };
            var onDescription = new Dictionary<string, object>
            {
                ["@type"] = "OnOffProperty",
                ["title"] = "On/Off",
                ["type"] = "boolean",
                ["description"] = "Whether the lamp is turned on"
            };

            var property = new Property<bool>(light, "on", true, onDescription);
            property.ValuedChanged += (sender, value) =>
            {
                Console.WriteLine($"On-State is now {value}");
            };

            light.AddProperty(property);

            var brightnessDescription = new Dictionary<string, object>
            {
                ["@type"] = "BrightnessProperty",
                ["title"] = "Brightness",
                ["type"] = "number",
                ["description"] = "The level of light from 0-100",
                ["minimum"] = 0,
                ["maximum"] = 100,
                ["unit"] = "percent"
            };

            var level = new Property<double>(light, "level", 0.0, brightnessDescription);
            level.ValuedChanged += (sender, value) =>
            {
                Console.WriteLine($"Brightness is now {value}");
            };

            light.AddProperty(level);

            services.AddThing(options => options.AddThing(light));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            app.UseThing();
        }
    }
}
