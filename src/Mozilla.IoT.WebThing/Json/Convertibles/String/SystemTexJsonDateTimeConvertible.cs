using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles.String
{
    /// <summary>
    /// Represent convertible/getter <see cref="string"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonDateTimeConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="SystemTexJsonDateTimeConvertible"/>
        /// </summary>
        public static SystemTexJsonDateTimeConvertible Instance { get; } = new SystemTexJsonDateTimeConvertible();
        
        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            result = null;
            
            if (source.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            if (!source.TryGetDateTime(out var value))
            {
                return false;
            }

            result = value;
            return true;
        }
    }
}
