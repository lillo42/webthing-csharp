using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mozilla.IoT.WebThing.Actions;

namespace Mozilla.IoT.WebThing.Converts
{
    public class StatusConverter : JsonConverter<ActionStatus>
    {
        private static readonly Type s_status = typeof(ActionStatus);
        
        public override bool CanConvert(Type typeToConvert) 
            => s_status == typeToConvert;

        public override ActionStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
            => Enum.Parse<ActionStatus>(reader.GetString(), true);

        public override void Write(Utf8JsonWriter writer, ActionStatus value, JsonSerializerOptions options) 
            => writer.WriteStringValue(value.ToString().ToLower());
    }
}
