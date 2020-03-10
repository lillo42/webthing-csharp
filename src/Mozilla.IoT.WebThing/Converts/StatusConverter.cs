using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mozilla.IoT.WebThing.Converts
{
    public class StatusConverter : JsonConverter<Status>
    {
        private static readonly Type s_status = typeof(Status);
        
        public override bool CanConvert(Type typeToConvert) 
            => s_status == typeToConvert;

        public override Status Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
            => Enum.Parse<Status>(reader.GetString(), true);

        public override void Write(Utf8JsonWriter writer, Status value, JsonSerializerOptions options) 
            => writer.WriteStringValue(value.ToString().ToLower());
    }
}
