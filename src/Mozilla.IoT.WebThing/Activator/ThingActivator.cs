using System;
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

        public void Register<T>(T thing, IServiceProvider service)
            where T : Thing

        {
            Register<T>(thing.Name);
            _typeActivatorCache.TryAdd(typeof(T), thing);
            BindingThingNotify(thing, service);
        }

        public void Register<T>(string thing) 
            where T : Thing 
            => _thingType.TryAdd(thing, typeof(T));

        public Thing CreateInstance(IServiceProvider serviceProvider, string thingName)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (thingName == null)
            {
                throw new ArgumentNullException(nameof(thingName));
            }

            if (!_thingType.ContainsKey(thingName))
            {
                return null;
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
    }
}
