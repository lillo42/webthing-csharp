using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles
{
    /// <summary>
    /// Represent convertible/getter <see cref="bool"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonArrayConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="SystemTexJsonArrayConvertible"/>
        /// </summary>
        public static SystemTexJsonArrayConvertible Instance { get; } = new SystemTexJsonArrayConvertible();
        
        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            if (source.ValueKind != JsonValueKind.Array)
            {
                result = null;
                return false;
            }
            
            var values = new object?[source.GetArrayLength()];

            var i = 0;
            foreach (var array in source.EnumerateArray())
            {
                values[i] = array.ValueKind switch
                {
                    JsonValueKind.String => array.GetString(),
                    JsonValueKind.Number => array.GetDecimal(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => null
                };
                i++;
            }

            result = values;
            return true;
        }
    }
}
