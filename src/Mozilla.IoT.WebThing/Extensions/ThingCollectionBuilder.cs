using System;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories;

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
                var factory = provider.GetRequiredService<IThingConverterFactory>();
                var options = provider.GetRequiredService<JsonSerializerOptions>();
                var converter = factory.Create(thing, options);
                
                thing.ThingContext = new ThingContext(converter, null);
                return thing;
            });
            
            _service.TryAddSingleton(service =>
            {
                var factory = service.GetRequiredService<IThingConverterFactory>();
                var options = service.GetRequiredService<JsonSerializerOptions>();
                return factory.Create(service.GetRequiredService<T>(), options);
            });
            return this;
        }

        public IThingCollectionBuilder AddThing<T>(T thing) 
            where T : Thing
        {
            _service.TryAddSingleton(thing);
            _service.TryAddSingleton<Thing>(provider =>
            {
                var factory = provider.GetRequiredService<IThingConverterFactory>();
                var options = provider.GetRequiredService<JsonSerializerOptions>();
                var converter = factory.Create(thing, options);
                
                thing.ThingContext = new ThingContext(converter, null);
                return thing;
            });

            return this;
        }
    }
}
