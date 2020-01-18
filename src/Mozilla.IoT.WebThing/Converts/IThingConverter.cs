using System.Text.Json;

namespace Mozilla.IoT.WebThing.Converts
{
    public interface IThingConverter
    {
        void Write(Utf8JsonWriter writer, Thing value, JsonSerializerOptions options);
    }
}
