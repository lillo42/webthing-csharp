using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Mozilla.IoT.WebThing
{
    internal sealed class ThingActivator : IThingActivator
    {
        private readonly Func<Type, ObjectFactory> _createFactory = type =>
            ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);

        private readonly ConcurrentDictionary<Type, Thing>
            _typeActivatorCache = new ConcurrentDictionary<Type, Thing>();

        public void RegisterInstance<T>(T thing) 
            where T : Thing 
            => _typeActivatorCache.TryAdd(typeof(T), thing);

        public Thing CreateInstance(IServiceProvider serviceProvider, Type implementationType)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (!_typeActivatorCache.ContainsKey(implementationType))
            {
                 var factory =  _createFactory(implementationType);
                 _typeActivatorCache.TryAdd(implementationType, (Thing)factory(serviceProvider, arguments: null));
            }

            return _typeActivatorCache[implementationType];
        }
    }
}
