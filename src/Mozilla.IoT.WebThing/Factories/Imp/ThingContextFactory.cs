using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc /> 
    public class ThingContextFactory : IThingContextFactory
    {
        private readonly IThingResponseBuilder _response;
        private readonly IEventBuilder _event;
        private readonly IPropertyBuilder _property;

        /// <summary>
        /// Initialize a new instance of <see cref="IEventBuilder"/>.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="property"></param>
        /// <param name="response"></param>
        public ThingContextFactory(IEventBuilder @event, IPropertyBuilder property, 
            IThingResponseBuilder response)
        {
            _event = @event;
            _property = property;
            _response = response;
        }

        /// <inheritdoc /> 
        public ThingContext Create(Thing thing, ThingOption option)
        {
            var thingType = thing.GetType();

            _response
                .SetThing(thing)
                .SetThingOption(option);
            
            _event
                .SetThing(thing)
                .SetThingOption(option)
                .SetThingType(thingType);

            _property
                .SetThing(thing)
                .SetThingOption(option);

            VisitEvent(thingType);
            VisitProperty(thingType);
            
            return new ThingContext(
                new Convert2(), 
                _event.Build(), 
                new Dictionary<string, ActionCollection>(), 
                _property.Build());
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
                _response.Add(@event, information);
            }
        }

        private void VisitProperty(Type thingType)
        {
            var properties = thingType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !IsThingProperty(x.Name));
            
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (!IsAcceptedType(propertyType))
                {
                    continue;
                }
                
                var attribute = property.GetCustomAttribute<ThingPropertyAttribute>();

                if (attribute != null && attribute.Ignore)
                {
                    continue;
                }
                
                var propertyName = attribute?.Name ?? property.Name;
                var isReadOnly = !property.CanWrite || !property.SetMethod!.IsPublic 
                                                    || (attribute != null && attribute.IsReadOnly);
                var isNullable = propertyType ==  typeof(string) || property.PropertyType.IsNullable();
                var information = ToInformation(propertyName, isReadOnly, isNullable, attribute, propertyType);
                
                _property.Add(property, information);
                _response.Add(property, attribute, information);
            }

            static Information ToInformation(string propertyName, bool isReadOnly, bool isNullable, 
                ThingPropertyAttribute? attribute, Type propertyType)
            {
                return new Information(attribute?.MinimumValue, attribute?.MaximumValue,
                    attribute?.ExclusiveMinimumValue, attribute?.ExclusiveMaximumValue,
                    attribute?.MultipleOfValue, attribute?.MinimumLengthValue, 
                    attribute?.MaximumLengthValue, attribute?.Pattern, GetEnums(propertyType, attribute?.Enum), 
                    isReadOnly, propertyName, isNullable);
            }
            
            static bool IsThingProperty(string name)
                => name == nameof(Thing.Context)
                   || name == nameof(Thing.Name)
                   || name == nameof(Thing.Description)
                   || name == nameof(Thing.Title)
                   || name == nameof(Thing.Type)
                   || name == nameof(Thing.ThingContext);
        }


        private static object[]? GetEnums(Type type, object[]? values)
        {
            if (type.IsEnum)
            {
                var enumValues = type.GetEnumValues();
                var result = new object[enumValues.Length];
                Array.Copy(enumValues, 0, result, 0, result.Length);
                return result;
            }
            
            return values;
        }
        
        private static bool IsAcceptedType(Type? type)
        {
            if (type == null)
            {
                return false;
            }

            type = type.GetUnderlyingType();

            return type == typeof(string)
                   || type == typeof(char)
                   || type == typeof(bool)
                   || type == typeof(int)
                   || type == typeof(byte)
                   || type == typeof(short)
                   || type == typeof(long)
                   || type == typeof(sbyte)
                   || type == typeof(uint)
                   || type == typeof(ulong)
                   || type == typeof(ushort)
                   || type == typeof(double)
                   || type == typeof(float)
                   || type == typeof(decimal)
                   || type == typeof(DateTime)
                   || type == typeof(DateTimeOffset)
                   || type == typeof(Guid)
                   || type == typeof(TimeSpan) 
                   || type.IsEnum;
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
