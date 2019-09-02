using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mozilla.IoT.WebThing.Accepted.Test.Startups
{
    public class SingleStartup
    {
        public SingleStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddThing(options => options.IsSingleThing = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            app.UseEndpoints(config =>
            {
                config.MapThing(new LightThing());
            });
        }
    }

    public sealed class LightThing : Thing
    {
        public override string Name { get; set; } = "My Lamp";
        public override object Type { get; set; } = new [] {"OnOffSwitch", "Light"};
        public override string Description { get; set; } = "A web connected lamp";

        public LightThing()
        {
            Properties.Add(new Property<bool>(this,
                "on",
                true,
                new Dictionary<string, object>()
                {
                    ["@type"] = "OnOffProperty",
                    ["title"] = "On/Off",
                    ["type"] = "boolean",
                    ["description"] = "Whether the lamp is turned on"
                }));

            Properties.Add(new Property<double>(this,
                "level",
                0,
                new Dictionary<string, object>()
                {
                    ["@type"] = "BrightnessProperty",
                    ["title"] = "Brightness",
                    ["type"] = "number",
                    ["description"] = "The level of light from 0-100",
                    ["minimum"] = 0,
                    ["maximum"] = 100,
                    ["unit"] = "percent"
                }));
            
            AddAction<FakeAction>("fake");
        }
    }

    public class FakeEvent : Event
    {
        public FakeEvent(Thing thing, string name, object data) 
            : base(thing, name, data)
        {
        }
    }

    public class FakeAction : Action
    {
        protected override ValueTask ExecuteAsync(CancellationToken cancellation)
            => new ValueTask(Task.Delay(3_000, cancellation));
    }
}
