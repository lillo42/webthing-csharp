using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles.String
{
    /// <summary>
    /// Represent convertible/getter <see cref="string"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonGuidConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="SystemTexJsonGuidConvertible"/>
        /// </summary>
        public static SystemTexJsonGuidConvertible Instance { get; } = new SystemTexJsonGuidConvertible();
        
        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            result = null;
            
            if (source.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            if (!source.TryGetGuid(out var value))
            {
                return false;
            }

            result = value;
            return true;

        }
    }
}
