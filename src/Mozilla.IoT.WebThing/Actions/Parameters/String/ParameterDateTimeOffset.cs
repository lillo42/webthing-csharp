using System;
using System.Linq;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions.Parameters.String
{
    /// <summary>
    /// Represent <see cref="DateTimeOffset"/> action parameter.
    /// </summary>
    public readonly struct ParameterDateTimeOffset : IActionParameter
    {
        private readonly DateTimeOffset[]? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="ParameterDateTimeOffset"/>.
        /// </summary>
        /// <param name="isNullable">If action parameter accepts null value.</param>
        /// <param name="enums">The possible values this action parameter can have.</param>
        public ParameterDateTimeOffset(bool isNullable, DateTimeOffset[]? enums)
        {
            CanBeNull = isNullable;
            _enums = enums;
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

            if (!element.TryGetDateTimeOffset(out var jsonValue))
            {
                return false;
            }

            if (_enums != null && _enums.Length > 0 &&  !_enums.Contains(jsonValue))
            {
                return false;
            }

            value = jsonValue;
            return true;
        }
    }
}
