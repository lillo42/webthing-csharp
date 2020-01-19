using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mozilla.IoT.WebThing.Converts
{
    public class ThingConverter : JsonConverter<Thing>
    {
        public ThingConverter(Dictionary<string, IThingConverter> thingConverts)
        {
        }

        public ThingConverter()
        {
            
            
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
            var builder = new UriBuilder(value.Context) {Path = $"/things/{value.Name}"};
            WriteProperty(writer, "Id", builder.Uri.ToString(), options);
            value.ThingContext.Converter.Write(writer, value, options);
            
            StartArray(writer, "Links", options);
            
            writer.WriteStartObject();
            WriteProperty(writer, "rel",  "properties", options);
            WriteProperty(writer, "href",  $"/things/{value.Name}/properties", options);
            writer.WriteEndObject();
            
            writer.WriteStartObject();
            WriteProperty(writer, "rel",  "actions", options);
            WriteProperty(writer, "href",  $"/things/{value.Name}/actions", options);
            writer.WriteEndObject();
            
            writer.WriteStartObject();
            WriteProperty(writer, "rel",  "events", options);
            WriteProperty(writer, "href",  $"/things/{value.Name}/events", options);
            writer.WriteEndObject();
            
            builder.Scheme = value.Prefix.Scheme == "http" ? "ws" : "wss";
            builder.Path = $"/things/{value.Name}";
            writer.WriteStartObject();
            WriteProperty(writer, "rel",  "alternate", options);
            WriteProperty(writer, "href",  builder.Uri.ToString(), options);
            writer.WriteEndObject();

            writer.WriteEndArray();
            
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

        private static void StartArray(Utf8JsonWriter writer, string propertyName, JsonSerializerOptions options)
        {
            var name = GetName(propertyName, options.PropertyNamingPolicy);
            writer.WriteStartArray(name);
        }
        
        #endregion
    }
}
