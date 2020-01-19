using System;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    internal sealed partial class ThingConverterGenerator
    {
        private void GenerateEvents(Thing thing, Type thingType)
        {
            var events = thingType.GetEvents(BindingFlags.Public | BindingFlags.Instance);
            if (events.Length == 0)
            {
                PropertyWithNullValue("Events");
                return;
            }
            StartObject("Events");

            foreach (var @event in events)
            {
                var information = @event.GetCustomAttribute<ThingEventAttribute>();
                
                if (information != null && information.Ignore)
                {
                    continue;
                }

                var name = information?.Name ?? @event.Name;
                StartObject(name);

                if (information != null)
                {
                    PropertyWithNullableValue(nameof(ThingEventAttribute.Title), information.Title);
                    PropertyWithNullableValue(nameof(ThingEventAttribute.Description), information.Description);
                    PropertyWithNullableValue(nameof(ThingEventAttribute.Unit), information.Unit);
                    PropertyType("@type", information.Type);
                }
                
                PropertyWithNullableValue("type", GetJsonType(GetEventType(@event.EventHandlerType)));
                
                StartArray("Links");
                StartObject();
                
                PropertyWithValue( "href", $"/things/{thing.Name}/events/{GetPropertyName(name)}");
                
                EndObject();
                EndArray();
                
                EndObject();
            }
            
            EndObject();


            static Type? GetEventType(Type eventHandlerType)
            {
                if (eventHandlerType == null 
                    || eventHandlerType.GenericTypeArguments.Length != 1)
                {
                    return null;
                }
                return eventHandlerType.GenericTypeArguments[0];
            }
        }
    }
}
