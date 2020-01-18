using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mozilla.IoT.WebThing.Converts
{
    public class ThingConverter : JsonConverter<Thing>
    {
        private readonly string _prefix;
        private readonly Dictionary<string, IThingConverter> _thingConverts;

        public ThingConverter(string prefix, Dictionary<string, IThingConverter> thingConverts)
        {
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            _thingConverts = thingConverts ?? throw new ArgumentNullException(nameof(thingConverts));
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsSubclassOf(typeof(Thing));
        }

        public override Thing Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Thing value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("@context", value.Context);
            WriteType(writer, value.Type, options);
            WriteProperty(writer, "Id", $"{_prefix}{value.Name}", options);
            WriteProperty(writer, nameof(Thing.Title), value.Title, options);
            WriteProperty(writer, nameof(Thing.Description), value.Description, options);
            _thingConverts[value.Name].Write(writer, value, options);
            
            writer.WriteEndObject();
        }

        #region Writer

        private static string GetName(string name, JsonNamingPolicy policy)
        {
            if (policy != null)
            {
                return policy.ConvertName(name);
            }

            return name;
        }

        private static void WriteProperty(Utf8JsonWriter writer, string name, string? value, JsonSerializerOptions options)
        {
            var propertyName = GetName(name, options.PropertyNamingPolicy);
            if (value == null)
            {
                if (!options.IgnoreNullValues)
                {
                    writer.WriteNull(propertyName);
                }
            }
            else
            {
                writer.WriteString(propertyName, value);
            }
        }

        private static void WriteType(Utf8JsonWriter writer, string[]? types, JsonSerializerOptions options)
        {
            if (types == null)
            {
                if (!options.IgnoreNullValues)
                {
                    writer.WriteNull("@type");
                }
            }
            else if (types.Length == 1)
            {
                writer.WriteString("@type", types[0]);
            }
            else
            {
                writer.WriteStartArray("@type");

                foreach (var type in types)
                {
                    writer.WriteStringValue(type);
                }

                writer.WriteEndArray();
            }
        }

        private static void WriteTitle(Utf8JsonWriter writer, string? title, JsonSerializerOptions options)
        {
            
        }

        private static void WriteDescription(Utf8JsonWriter writer, string? description, JsonSerializerOptions options)
        {
            var propertyName = options.PropertyNamingPolicy.ConvertName(nameof(Thing.Description));
            if (description == null)
            {
                if (!options.IgnoreNullValues)
                {
                    writer.WriteNull(propertyName);
                }
            }
            else
            {
                writer.WriteString(propertyName, description);
            }
        }

        #endregion
    }
}
