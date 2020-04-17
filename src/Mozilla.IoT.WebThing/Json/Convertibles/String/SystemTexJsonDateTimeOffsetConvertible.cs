using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles.String
{
    /// <summary>
    /// Represent convertible/getter <see cref="string"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonDateTimeOffsetConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="SystemTexJsonDateTimeOffsetConvertible"/>
        /// </summary>
        public static SystemTexJsonDateTimeOffsetConvertible Instance { get; } = new SystemTexJsonDateTimeOffsetConvertible();
        
        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            result = null;
            
            if (source.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            if (!source.TryGetDateTimeOffset(out var value))
            {
                return false;
            }

            result = value;
            return true;

        }
    }
}
