using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles.String
{
    /// <summary>
    /// Represent convertible/getter <see cref="string"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonCharConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="SystemTexJsonCharConvertible"/>
        /// </summary>
        public static SystemTexJsonCharConvertible Instance { get; } = new SystemTexJsonCharConvertible();
        
        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            if (source.ValueKind != JsonValueKind.String)
            {
                result = null;
                return false;
            }

            var value = source.GetString();

            if (value.Length != 1)
            {
                result = null;
                return false;
            }

            result = value[0];
            return true;
        }
    }
}
