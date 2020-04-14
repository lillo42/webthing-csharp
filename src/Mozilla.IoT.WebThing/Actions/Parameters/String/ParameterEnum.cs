using System;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions.Parameters.String
{
    /// <summary>
    /// Represent <see cref="Enum"/> action parameter.
    /// </summary>
    public readonly struct ParameterEnum : IActionParameter
    {
        private readonly Type _enumType;

        /// <summary>
        /// Initialize a new instance of <see cref="ParameterDateTime"/>.
        /// </summary>
        /// <param name="isNullable">If action parameter accepts null value.</param>
        /// <param name="enumType">The enum type.</param>
        public ParameterEnum(bool isNullable, Type enumType)
        {
            CanBeNull = isNullable;
            _enumType = enumType;
        }

        /// <inheritdoc/>
        public bool CanBeNull { get; }

        /// <inheritdoc/>
        public bool TryGetValue(JsonElement element, out object? value)
        {
            value = null;
            if (CanBeNull && element.ValueKind == JsonValueKind.Null)
            {
                return true;
            }
            
            if (element.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            if(!Enum.TryParse(_enumType, element.GetString(), true, out var jsonValue))
            {
                return false;
            }
            
            value = jsonValue;
            return true;
        }
    }
}
