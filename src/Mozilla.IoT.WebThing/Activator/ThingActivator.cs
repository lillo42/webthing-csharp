using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.Descriptor;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.Notify;

namespace Mozilla.IoT.WebThing.Activator
{
    internal sealed class ThingActivator : IThingActivator
    {
        private readonly ThingBindingOption _option;
        private readonly Func<Type, ObjectFactory> _createFactory = type =>
            ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);
#if DEBUG
        internal readonly Dictionary<string, Type> _thingType = new Dictionary<string, Type>();
        
        internal readonly ConcurrentDictionary<Type, Thing>
            _typeActivatorCache = new ConcurrentDictionary<Type, Thing>();
#else
        private readonly Dictionary<string, Type> _thingType = new Dictionary<string, Type>();
        
        private readonly ConcurrentDictionary<Type, Thing>
            _typeActivatorCache = new ConcurrentDictionary<Type, Thing>();
#endif
        public ThingActivator(ThingBindingOption option)
        {
            _option = option;
        }

        public void Register<T>(IServiceProvider service) where T : Thing
        {
            string name = typeof(T).Name;
            Register<T>(service, name.Replace("Thing", ""));

            var thing = CreateInstance(service, name.Replace("Thing", ""));

            if (thing.Name != null)
            {
                _thingType.TryAdd(thing.Name, typeof(T));
            }
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
            _thingType.TryAdd(thing.Name, typeof(T));
            _typeActivatorCache.TryAdd(typeof(T), thing);
            
            BindingThingNotify(thing, service, thing.Name);
        }


        public Thing CreateInstance(IServiceProvider serviceProvider, string thingName)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (thingName == null)
            {
                if (_thingType.Count != 1)
                {
                    throw new ArgumentNullException(nameof(thingName));
                }

                thingName = _thingType.First().Key;
            }

            var implementationType = _thingType[thingName];

            if (!_typeActivatorCache.ContainsKey(implementationType))
            {
                var factory = _createFactory(implementationType);
                var instance = (Thing)factory(serviceProvider, null);
                if (_typeActivatorCache.TryAdd(implementationType, instance))
                {
                    BindingThingNotify(_typeActivatorCache[implementationType], serviceProvider, thingName);
                }
            }

            return _typeActivatorCache[implementationType];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BindingThingNotify(Thing thing, IServiceProvider serviceProvider, string thingName)
        {
            var eventDescription = serviceProvider.GetService<IDescriptor<Event>>();
            var jsonConvert = serviceProvider.GetService<IJsonSerializer>();
            var jsonSchemaValidator = serviceProvider.GetService<IJsonSchemaValidator>();
            var jsonValue = serviceProvider.GetService<IJsonValue>();

            var eventNotify = new NotifySubscribesOnEventAdded(thing,
                eventDescription,
                jsonConvert
            );

            var actionNotify = new NotifySubscribesOnActionStatusChange(serviceProvider.GetService<IDescriptor<Action>>(),
                jsonConvert
            );
            
            var propertyNotify = new NotifySubscribesOnPropertyChanged(jsonConvert);

            if (thing.Events == null)
            {
                thing.Events = serviceProvider.GetService<IEventCollection>();
            }

            thing.Events.EventAdded += eventNotify.Notify;
            thing.Actions.ActionStatusChanged += actionNotify.Notify;
            thing.Properties.ValueChanged += propertyNotify.Notify;
            thing.Properties.JsonSchemaValidator = jsonSchemaValidator;
            thing.Properties.JsonValue = jsonValue;


            if (!_option.IsSingleThing)
            {
                thing.HrefPrefix = $"/things/{thingName}";
            }
        }

        public IEnumerator<Thing> GetEnumerator() 
            => _typeActivatorCache.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }
}
