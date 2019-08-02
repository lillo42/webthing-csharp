using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Description;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.Notify;

namespace Mozilla.IoT.WebThing
{
    internal sealed class ThingActivator : IThingActivator
    {
        private readonly Func<Type, ObjectFactory> _createFactory = type =>
            ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);

        private readonly Dictionary<string, Type> _thingType = new Dictionary<string, Type>();

        private readonly ConcurrentDictionary<Type, Thing>
            _typeActivatorCache = new ConcurrentDictionary<Type, Thing>();
        
        public void Register<T>(IServiceProvider service) where T : Thing
        {
            string name = typeof(T).Name;
            Register<T>(service, name.Remove(name.Length - 5));
        }

        public void Register<T>(IServiceProvider service, string thing)
            where T : Thing
        {
            _thingType.TryAdd(thing, typeof(T));
            CreateInstance(service, thing);
        }

        public void Register<T>(IServiceProvider service, T thing)
            where T : Thing

        {
            _typeActivatorCache.TryAdd(typeof(T), thing);
            BindingThingNotify(thing, service);
        }


        public Thing CreateInstance(IServiceProvider serviceProvider, string thingName)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (thingName == null)
            {
                if (_thingType.Count > 1)
                {
                    throw new ArgumentNullException(nameof(thingName));
                }

                thingName = _thingType.First().Key;
            }

            Type implementationType = _thingType[thingName];

            if (!_typeActivatorCache.ContainsKey(implementationType))
            {
                var factory = _createFactory(implementationType);
                if (_typeActivatorCache.TryAdd(implementationType, (Thing)factory(serviceProvider, arguments: null)))
                {
                    BindingThingNotify(_typeActivatorCache[implementationType], serviceProvider);
                }
            }

            return _typeActivatorCache[implementationType];
        }

        private static void BindingThingNotify(Thing thing, IServiceProvider serviceProvider)
        {
            var eventDescription = serviceProvider.GetService<IDescription<Event>>();
            var jsonConvert = serviceProvider.GetService<IJsonConvert>();
            var jsonSettings = serviceProvider.GetService<IJsonSerializerSettings>();
            var jsonSchemaValidator = serviceProvider.GetService<IJsonSchemaValidator>();

            var eventNotify = new NotifySubscribesOnEventAdded(thing,
                eventDescription,
                jsonConvert,
                jsonSettings
            );

            var actionNotify = new NotifySubscribesOnActionAdded(serviceProvider.GetService<IDescription<Action>>(),
                jsonConvert,
                jsonSettings
            );

            var propertyNotify = new NotifySubscribesOnPropertyChanged(jsonConvert, jsonSettings);

            if (thing.Events == null)
            {
                thing.Events = serviceProvider.GetService<IObservableCollection<Event>>();
            }

            ((PropertyCollection) thing.Properties).JsonSchemaValidator = jsonSchemaValidator;

            thing.Events.CollectionChanged += eventNotify.Notify;
            thing.Actions.CollectionChanged += actionNotify.Notify;
            thing.Properties.Cast<PropertyProxy>().ForEach(property =>
            {
                property.SchemaValidator = jsonSchemaValidator;
                property.ValuedChanged += propertyNotify.Notify;
            });
        }

        public IEnumerator<Thing> GetEnumerator() 
            => _typeActivatorCache.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }
}
