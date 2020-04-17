using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles.String
{
    /// <summary>
    /// Represent convertible/getter <see cref="string"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonTimeSpanConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="SystemTexJsonTimeSpanConvertible"/>
        /// </summary>
        public static SystemTexJsonTimeSpanConvertible Instance { get; } = new SystemTexJsonTimeSpanConvertible();
        
        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            result = null;
            
            if (source.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            if (!TimeSpan.TryParse(source.GetString(), out var value))
            {
                return false;
            }

            result = value;
            return true;

        }
    }
}
