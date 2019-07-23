using System.Text.Json.Serialization;

namespace Mozilla.IoT.WebThing.Json
{
    public class DefaultJsonSerializerSettings : IJsonSerializerSettings
    {
        public DefaultJsonSerializerSettings(JsonSerializerOptions options)
        {
            Options = options;
        }

        public JsonSerializerOptions Options { get; }

        public bool IgnoreNullValues => Options.IgnoreNullValues;
        public bool WriteIndented => Options.WriteIndented;
    }
}
