using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mozilla.IoT.WebThing.Converts
{
    public class ThingConverter : JsonConverter<Thing>
    {
        public override Thing Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Thing value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            
            WriteContext(writer, value.Context);
            WriteType(writer, value.Type);
            WriteTitle(writer, value.Title);
            WriteDescription(writer, value.Description);
            
            writer.WriteEndObject();
            throw new NotImplementedException();
        }

        private static void WriteContext(Utf8JsonWriter writer, string context)
        {
            writer.WritePropertyName("@context");
            writer.WriteStringValue(context);
        }
        private static void WriteType(Utf8JsonWriter writer, string[]? types)
        {
            writer.WritePropertyName("@type");

            if (types == null )
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
        private static void WriteTitle(Utf8JsonWriter writer, string title)
        {
            writer.WritePropertyName(nameof(Thing.Title));
            if (title == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(title);
            }
        }
        private static void WriteDescription(Utf8JsonWriter writer, string description)
        {
            writer.WritePropertyName(nameof(Thing.Description));
            if (description == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(description);
            }
        }
    }
}
