using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Properties;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc /> 
    public class ThingContextFactory : IThingContextFactory
    {
        private readonly IEventBuilder _event;

        /// <summary>
        /// Initialize a new instance of <see cref="IEventBuilder"/>.
        /// </summary>
        /// <param name="event"></param>
        public ThingContextFactory(IEventBuilder @event)
        {
            _event = @event;
        }

        /// <inheritdoc /> 
        public ThingContext Create(Thing thing, ThingOption option)
        {
            var thingType = thing.GetType();

            _event
                .SetThing(thing)
                .SetThingOption(option)
                .SetThingType(thingType);
            
            VisitEvent(thingType);
            
            return new ThingContext(
                new Convert2(), 
                _event.Build(), 
                new Dictionary<string, ActionCollection>(), 
                new Dictionary<string, IProperty>());
        }

        private static readonly Type s_eventHandler = typeof(EventHandler);
        private static readonly Type s_eventHandlerGeneric = typeof(EventHandler<>);
        
        private void VisitEvent(Type thingType)
        {
            var events = thingType.GetEvents(BindingFlags.Public | BindingFlags.Instance);

            foreach (var @event in events)
            {
                var args = @event.EventHandlerType!.GetGenericArguments();
                if (args.Length > 1)
                {
                    continue;
                }
                
                if ((args.Length == 0 && @event.EventHandlerType != s_eventHandler)
                    || (args.Length == 1 && @event.EventHandlerType != s_eventHandlerGeneric.MakeGenericType(args[0])))
                {
                    continue;
                }
                
                var information = @event.GetCustomAttribute<ThingEventAttribute>();

                if (information != null && information.Ignore)
                {
                    continue;
                }
                
                _event.Add(@event, information);
            }
        }
    }

    public class Convert2 : IThingConverter
    {
        public void Write(Utf8JsonWriter writer, Thing value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
