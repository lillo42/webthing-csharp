using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json
{
    public class JsonValue : IJsonValue
    {
        public object GetValue(object value, Type type)
        {
            if (value is JsonElement element)
            {
                return System.Text.Json.JsonSerializer.Deserialize(element.GetRawText(), type);
            }

            return value;
        }
    }
}
