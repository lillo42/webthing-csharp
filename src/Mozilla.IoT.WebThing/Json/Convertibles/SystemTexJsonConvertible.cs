
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles
{
    /// <summary>
    /// Represent convertible/getter value from <see cref="JsonElement"/>.
    /// </summary>
    public abstract class SystemTexJsonConvertible : IJsonConvertible
    {

        /// <inheritdoc/>
        public bool TryConvert(object source, out object? result)
        {
            result = null;
            var jsonElement = (JsonElement)source;
            return jsonElement.ValueKind == JsonValueKind.Null || TryConvert(jsonElement, out result);
        }

        
        /// <summary>
        /// Try convert json value to specific value.
        /// </summary>
        /// <param name="source">The <see cref="JsonElement"/>.</param>
        /// <param name="result">The result.</param>
        /// <returns>return true if could get value, otherwise return false.</returns>
        protected abstract bool TryConvert(JsonElement source, out object? result);
    }
}
