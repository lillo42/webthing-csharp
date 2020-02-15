using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Factories.Generator.Actions;
using Mozilla.IoT.WebThing.Factories.Generator.Converter;
using Mozilla.IoT.WebThing.Factories.Generator.Events;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;
using Mozilla.IoT.WebThing.Factories.Generator.Properties;

namespace Mozilla.IoT.WebThing.Extensions
{
    public class ThingCollectionBuilder : IThingCollectionBuilder
    {
        private readonly IServiceCollection _service;

        public ThingCollectionBuilder(IServiceCollection service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public IThingCollectionBuilder AddThing<T>() 
            where T : Thing
        {
            _service.AddSingleton<T>();
            _service.AddSingleton(ConfigureThing<T>);
            return this;
        }

        public IThingCollectionBuilder AddThing<T>(T thing) 
            where T : Thing
        {
            if (thing == null)
            {
                throw new ArgumentNullException(nameof(thing));
            }

            _service.AddSingleton(thing);
            _service.AddSingleton(ConfigureThing<T>);

            return this;
        }

        private static Thing ConfigureThing<T>(IServiceProvider provider)
            where T : Thing
        {
            var thing = provider.GetRequiredService<T>();
            var option = provider.GetRequiredService<ThingOption>();
            var optionsJson = new JsonSerializerOptions
            {
                WriteIndented = false,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var converter = new ConverterInterceptorFactory(thing, optionsJson);
            var properties = new PropertiesInterceptFactory(thing, option);
            var events = new EventInterceptFactory(thing, option);
            var actions = new ActionInterceptFactory(option);
                
            CodeGeneratorFactory.Generate(thing, new List<IInterceptorFactory>()
            {
                converter,
                properties, 
                events,
                actions
            });

            thing.ThingContext = new Context(converter.Create(), 
                properties.Create(), 
                events.Events,
                actions.Actions);
            return thing;
            
        }
    }
}
