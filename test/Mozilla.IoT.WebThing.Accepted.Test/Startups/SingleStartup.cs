using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

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
            services.AddThing();
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

            app.UseSingleThing(new LightThing());
        }
    }

    public sealed class LightThing : Thing
    {
        public LightThing()
            : base("My Lamp", new JArray("OnOffSwitch", "Light"), "A web connected lamp")
        {
            AddProperty(new Property<bool>(this,
                "on",
                true,
                new JObject
                {
                    {"@type", "OnOffProperty"},
                    {"title", "On/Off"},
                    {"type", "boolean"},
                    {"description", "Whether the lamp is turned on"}
                }));

            AddProperty(new Property<double>(this,
                "level",
                0,
                new JObject
                {
                    {"@type", "BrightnessProperty"},
                    {"title", "Brightness"},
                    {"type", "number"},
                    {"description", "The level of light from 0-100"},
                    {"minimum", 0},
                    {"maximum", 100},
                    {"unit", "percent"}
                }));
            
            AddAvailableAction<FakeAction>("fake");
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
        public override string Id { get; } = Guid.NewGuid().ToString();
        public override string Name { get; } = nameof(FakeAction);
    }

}
