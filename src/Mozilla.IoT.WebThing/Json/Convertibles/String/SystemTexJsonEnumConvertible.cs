using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json.Convertibles.String
{
    /// <summary>
    /// Represent convertible/getter <see cref="string"/> from <see cref="JsonElement"/>.
    /// </summary>
    public class SystemTexJsonEnumConvertible : SystemTexJsonConvertible
    {
        private readonly Type _enum;

        /// <summary>
        /// Initialize a new instance of <see cref="SystemTexJsonEnumConvertible"/>.
        /// </summary>
        /// <param name="enum">The enum type</param>
        public SystemTexJsonEnumConvertible(Type @enum)
        {
            _enum = @enum ?? throw new ArgumentNullException(nameof(@enum));
        }

        /// <inheritdoc/>
        protected override bool TryConvert(JsonElement source, out object? result)
        {
            result = null;
            
            if (source.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            if (!Enum.TryParse(_enum, source.GetString(), out var value))
            {
                return false;
            }

            result = value;
            return true;

        }
    }
}
