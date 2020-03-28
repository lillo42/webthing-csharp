using System;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Factories;

namespace Mozilla.IoT.WebThing.Extensions
{
    /// <inheritdoc />
    public class ThingCollectionBuilder : IThingCollectionBuilder
    {
        private readonly IServiceCollection _service;

        internal ThingCollectionBuilder(IServiceCollection service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <inheritdoc />
        public IThingCollectionBuilder AddThing<T>() 
            where T : Thing
        {
            _service.AddSingleton<T>();
            _service.AddSingleton(ConfigureThing<T>);
            return this;
        }
        
        /// <inheritdoc />
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
            var factory = provider.GetRequiredService<IThingContextFactory>();
            
            thing.ThingContext = factory.Create(thing, option);
            
            return thing;
            
        }
    }
}
