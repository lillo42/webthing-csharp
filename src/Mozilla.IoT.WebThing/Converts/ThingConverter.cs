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
            WriteType(writer, value.Type);
            writer.WriteString("Id", $"{_prefix}{value.Name}");
            WriteTitle(writer, value.Title);
            WriteDescription(writer, value.Description);
            _thingConverts[value.Name].Write(writer, value, options);
            
            writer.WriteEndObject();
        }

        #region Writer

        private static void WriteType(Utf8JsonWriter writer, string[]? types)
        {
            writer.WritePropertyName("@type");

            if (types == null)
            {
                writer.WriteNullValue();
            }
            else if (types.Length == 1)
            {
                writer.WriteStringValue(types[0]);
            }
            else
            {
                writer.WriteStartArray();

                foreach (var type in types)
                {
                    writer.WriteStringValue(type);
                }

                writer.WriteEndArray();
            }
        }

        private static void WriteTitle(Utf8JsonWriter writer, string? title)
        {
            if (title == null)
            {
                writer.WriteNull(nameof(Thing.Title));
            }
            else
            {
                writer.WriteString(nameof(Thing.Title), title);
            }
        }

        private static void WriteDescription(Utf8JsonWriter writer, string? description)
        {
            if (description == null)
            {
                writer.WriteNull(nameof(Thing.Description));
            }
            else
            {
                writer.WriteString(nameof(Thing.Description), description);
            }
        }

        #endregion
    }
}
