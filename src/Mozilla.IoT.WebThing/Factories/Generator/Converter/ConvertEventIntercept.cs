using System;
using System.Reflection;
using System.Text.Json;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Converter
{
    internal class ConvertEventIntercept : IEventIntercept
    {
        private readonly Utf8JsonWriterILGenerator _jsonWriter;
        private readonly JsonSerializerOptions _options;
        private bool _isObjectStart;

        public ConvertEventIntercept(Utf8JsonWriterILGenerator jsonWriter, JsonSerializerOptions options)
        {
            _jsonWriter = jsonWriter ?? throw new ArgumentNullException(nameof(jsonWriter));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Before(Thing thing)
        {
            
        }

        public void After(Thing thing)
        {
            if (_isObjectStart)
            {
                _jsonWriter.EndObject();
            }
            else
            {
                _jsonWriter.PropertyWithNullValue("Events");
            }
        }

        public void Visit(Thing thing, EventInfo @event, ThingEventAttribute? eventInfo)
        {
            if (!_isObjectStart)
            {
                _jsonWriter.StartObject("Events");
                _isObjectStart = true;
            }
            
            var name = eventInfo?.Name ?? @event.Name;
            _jsonWriter.StartObject(name);

            if (eventInfo != null)
            {
                _jsonWriter.PropertyWithNullableValue(nameof(ThingEventAttribute.Title), eventInfo.Title);
                _jsonWriter.PropertyWithNullableValue(nameof(ThingEventAttribute.Description), eventInfo.Description);
                _jsonWriter.PropertyWithNullableValue(nameof(ThingEventAttribute.Unit), eventInfo.Unit);
                _jsonWriter.PropertyType("@type", eventInfo.Type);
            }
                
            _jsonWriter.PropertyWithNullableValue("type", GetJsonType(GetEventType(@event.EventHandlerType)));
                
            _jsonWriter.StartArray("Links");
            _jsonWriter.StartObject();
                
            _jsonWriter.PropertyWithValue( "href", $"/things/{thing.Name}/events/{_options.GetPropertyName(name)}");
                
            _jsonWriter.EndObject();
            _jsonWriter.EndArray();
                
            _jsonWriter.EndObject();
            
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

        private static string? GetJsonType(Type? type)
        {
            if (type == null)
            {
                return null;
            }

            if (type == typeof(string)
                || type == typeof(DateTime))
            {
                return "string";
            }

            if (type == typeof(bool))
            {
                return "boolean";
            }
            
            if (type == typeof(int)
                || type == typeof(sbyte)
                || type == typeof(byte)
                || type == typeof(short)
                || type == typeof(long)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(ushort))
            {
                return "integer";
            }
            
            if (type == typeof(double)
                || type == typeof(float))
            {
                return "number";
            }

            return null;
        }
    }
}
