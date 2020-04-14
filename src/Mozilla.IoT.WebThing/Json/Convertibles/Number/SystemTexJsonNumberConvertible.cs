using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles
{
    /// <summary>
    /// Represent convertible/getter <see cref="bool"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonNumberConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="SystemTexJsonNumberConvertible"/>
        /// </summary>
        public static SystemTexJsonNumberConvertible Instance { get; } = new SystemTexJsonNumberConvertible();

        
        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            if (source.ValueKind != JsonValueKind.Number || !source.TryGetDecimal(out var number))
            {
                result = null;
                return false;
            }

            result = number;
            return true;
        }
    }
}
