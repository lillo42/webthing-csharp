using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories;
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
            _service.TryAddSingleton<T>();
            _service.TryAddSingleton<Thing>(provider =>
            {
                var thing = provider.GetRequiredService<T>();
                var options = provider.GetRequiredService<JsonSerializerOptions>();

                var converter = new ConverterInterceptorFactory(thing, options);
                var properties = new PropertiesInterceptFactory(thing, options);
                var events = new EventInterceptFactory(thing, options);
                CodeGeneratorFactory.Generate(thing, options, new List<IInterceptorFactory>()
                {
                    converter,
                    properties, 
                    events
                });

                thing.ThingContext = new ThingContext(converter.Create(), properties.Create(), events.Events);
                return thing;
            });
            return this;
        }

        public IThingCollectionBuilder AddThing<T>(T thing) 
            where T : Thing
        {
            if (thing == null)
            {
                throw new ArgumentNullException(nameof(thing));
            }

            _service.TryAddSingleton(thing);
            _service.TryAddSingleton<Thing>(provider =>
            {
                var thing = provider.GetRequiredService<T>();
                var options = provider.GetRequiredService<JsonSerializerOptions>();

                var converter = new ConverterInterceptorFactory(thing, options);
                var properties = new PropertiesInterceptFactory(thing, options);
                var events = new EventInterceptFactory(thing, options);
                
                CodeGeneratorFactory.Generate(thing, options, new List<IInterceptorFactory>()
                {
                    converter,
                    properties, 
                    events
                });

                thing.ThingContext = new ThingContext(converter.Create(), properties.Create(), events.Events);
                return thing;
            });

            return this;
        }
    }
}
