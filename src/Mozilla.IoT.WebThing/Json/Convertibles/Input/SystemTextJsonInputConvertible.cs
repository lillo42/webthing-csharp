using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles.Input
{
    /// <summary>
    /// Represent convertible/getter <see cref="string"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTextJsonInputConvertible : SystemTexJsonConvertible
    {
        private readonly Dictionary<string, IJsonConvertible> _convertibles;

        /// <summary>
        /// Initialize a new instance of <see cref="SystemTextJsonInputConvertible"/>.
        /// </summary>
        /// <param name="convertibles">The convertibles.</param>
        public SystemTextJsonInputConvertible(Dictionary<string, IJsonConvertible> convertibles)
        {
            _convertibles = convertibles;
        }

        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            result = null;
            if (!source.TryGetProperty("input", out var input))
            {
                return false;
            }

            if (input.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            var dict = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var property in input.EnumerateObject())
            {
                if (!_convertibles.TryGetValue(property.Name, out var convertible))
                {
                    return false;
                }

                if (!convertible.TryConvert(property.Value, out var value))
                {
                    return false;
                }
                
                dict.Add(property.Name, value);
            }
            
            result = dict;
            return true;
        }
    }
}
