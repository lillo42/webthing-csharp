using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Events
{
    public class EventInterceptFactory : IInterceptorFactory
    {
        private readonly EventIntercept _intercept;

        public EventInterceptFactory(Thing thing, JsonSerializerOptions options)
        {
            var thingType = thing.GetType();
            var builder = Factory.CreateTypeBuilder($"{thingType.Name}EventBinder", thingType.Name, 
                null, TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public);
            
            _intercept = new EventIntercept(options, builder, Events);
        }

        public Dictionary<string, ThingEventCollection> Events { get; } = new Dictionary<string, ThingEventCollection>();

        public IThingIntercept CreateThingIntercept()
            => new EmptyIntercept();

        public IPropertyIntercept CreatePropertyIntercept()
            => new EmptyIntercept();

        public IActionIntercept CreatActionIntercept()
            => new EmptyIntercept();

        public IEventIntercept CreatEventIntercept()
            => _intercept;
    }
}
