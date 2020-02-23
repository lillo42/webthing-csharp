using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Converts
{
    public class ThingConverter : JsonConverter<Thing>
    {

        private readonly ThingOption _option;
        public ThingConverter(ThingOption option)
        {
            _option = option;
        }
        
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Thing) || typeToConvert.IsSubclassOf(typeof(Thing));
        }

        public override Thing Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Thing value, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            
            writer.WriteStartObject();
            writer.WriteString("@context", value.Context);
            var builder = new UriBuilder(value.Prefix) {Path = $"/things/{options.GetPropertyName(value.Name)}"};
            if (_option.UseThingAdapterUrl)
            {
                WriteProperty(writer, "Id", options.GetPropertyName(value.Name), options);
                WriteProperty(writer, "href", builder.Path, options);
                WriteProperty(writer, "base", builder.Uri.ToString(), options);
            }
            else
            {
                WriteProperty(writer, "Id", builder.Uri.ToString(), options);
            }

            value.ThingContext.Converter.Write(writer, value, options);
            
            StartArray(writer, "Links", options);
            
            writer.WriteStartObject();
            WriteProperty(writer, "rel",  "properties", options);
            WriteProperty(writer, "href",  $"/things/{options.GetPropertyName(value.Name)}/properties", options);
            writer.WriteEndObject();
            
            writer.WriteStartObject();
            WriteProperty(writer, "rel",  "actions", options);
            WriteProperty(writer, "href",  $"/things/{options.GetPropertyName(value.Name)}/actions", options);
            writer.WriteEndObject();
            
            writer.WriteStartObject();
            WriteProperty(writer, "rel",  "events", options);
            WriteProperty(writer, "href",  $"/things/{options.GetPropertyName(value.Name)}/events", options);
            writer.WriteEndObject();
            
            builder.Scheme = value.Prefix.Scheme == "http" ? "ws" : "wss";
            writer.WriteStartObject();
            WriteProperty(writer, "rel",  "alternate", options);
            WriteProperty(writer, "href",  builder.Uri.ToString(), options);
            writer.WriteEndObject();

            writer.WriteEndArray();
            
            writer.WriteEndObject();
        }

        #region Writer

        private static string GetName(string name, JsonNamingPolicy policy) 
            => policy != null ? policy.ConvertName(name) : name;

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
