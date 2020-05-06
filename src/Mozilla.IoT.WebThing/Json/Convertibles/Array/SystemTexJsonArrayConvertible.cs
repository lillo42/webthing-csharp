using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles
{
    /// <summary>
    /// Represent convertible/getter <see cref="bool"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonArrayConvertible : SystemTexJsonConvertible
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="convertible"></param>
        public SystemTexJsonArrayConvertible(IJsonConvertible convertible)
        {
            _convertible = convertible ?? throw new ArgumentNullException(nameof(convertible));
        }

        private readonly IJsonConvertible _convertible;

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
                if (!_convertible.TryConvert(array, out var value))
                {
                    result = null;
                    return false;
                }
                
                values[i] = value;
                i++;
            }

            result = values;
            return true;
        }
    }
}
