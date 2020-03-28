using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Mozilla.IoT.WebThing.Actions.Parameters.String
{
    /// <summary>
    /// Represent <see cref="string"/> action parameter.
    /// </summary>
    public readonly struct ParameterString : IActionParameter
    {
        private readonly int? _minimum;
        private readonly int? _maximum;
        private readonly string[]? _enums;
        private readonly Regex? _pattern;

        /// <summary>
        /// Initialize a new instance of <see cref="ParameterString"/>.
        /// </summary>
        /// <param name="isNullable">If action parameter accepted null value.</param>
        /// <param name="minimum">The minimum length of string to be assign.</param>
        /// <param name="maximum">The maximum length of string to be assign.</param>
        /// <param name="pattern">The pattern of string to be assign.</param>
        /// <param name="enums">The possible values this action parameter could have.</param>
        public ParameterString(bool isNullable, int? minimum, int? maximum, string? pattern, string[]? enums)
        {
            CanBeNull = isNullable;
            _minimum = minimum;
            _maximum = maximum;
            _enums = enums;
            _pattern = pattern != null ? new Regex(pattern, RegexOptions.Compiled) : null;
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

            var jsonValue = element.GetString();
            
            if (_minimum.HasValue && jsonValue.Length < _minimum.Value)
            {
                return false;
            }
            
            if (_maximum.HasValue && jsonValue.Length > _maximum.Value)
            {
                return false;
            }

            if (_enums != null && _enums.Length > 0 && !_enums.Contains(jsonValue))
            {
                return false;
            }

            if (_pattern != null && !_pattern.IsMatch(jsonValue))
            {
                return false;
            }

            value = jsonValue;
            return true;
        }
    }
}
