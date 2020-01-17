using System.Text.Json;

namespace Mozilla.IoT.WebThing.Converts
{
    public interface IThingConverter
    {
        void Write(Utf8JsonWriter writer, Thing value, JsonSerializerOptions options);
    }

    public interface IThingConverter<T> : IThingConverter
        where T : Thing
    {
        void IThingConverter.Write(Utf8JsonWriter writer, Thing value, JsonSerializerOptions options)
            => Write(writer, (T)value, options);

        void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
    }
}
