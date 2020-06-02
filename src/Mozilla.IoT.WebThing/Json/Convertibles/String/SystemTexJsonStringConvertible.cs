using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles.String
{
    /// <summary>
    /// Represent convertible/getter <see cref="string"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonStringConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="SystemTexJsonStringConvertible"/>
        /// </summary>
        public static SystemTexJsonStringConvertible Instance { get; } = new SystemTexJsonStringConvertible();
        
        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            if (source.ValueKind != JsonValueKind.String)
            {
                result = null;
                return false;
            }

            result = source.GetString();
            return true;
        }
    }
}
