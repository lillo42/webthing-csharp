using System.Text.Json;

namespace Mozilla.IoT.WebThing.Converts
{
    /// <summary>
    /// Convert Thing
    /// </summary>
    public interface IThingConverter
    {
        /// <summary>
        /// Convert Thing to JsonElement
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/>.</param>
        /// <param name="value">The <see cref="Thing"/>.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/>.</param>
        void Write(Utf8JsonWriter writer, Thing value, JsonSerializerOptions options);
    }
}
