using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles
{
    /// <summary>
    /// Represent convertible/getter <see cref="bool"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonBooleanConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="SystemTexJsonBooleanConvertible"/>
        /// </summary>
        public static SystemTexJsonBooleanConvertible Instance { get; } = new SystemTexJsonBooleanConvertible();

        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            if (source.ValueKind != JsonValueKind.True && source.ValueKind != JsonValueKind.False)
            {
                result = null;
                return false;
            }

            result = source.GetBoolean();
            return true;
        }
    }
}
