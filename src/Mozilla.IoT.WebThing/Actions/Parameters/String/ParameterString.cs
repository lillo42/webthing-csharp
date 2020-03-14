using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Mozilla.IoT.WebThing.Actions.Parameters.String
{
    public readonly struct ParameterString : IActionParameter
    {
        private readonly int? _minimum;
        private readonly int? _maximum;
        private readonly string[]? _enums;
        private readonly Regex? _pattern;

        public ParameterString(bool isNullable, int? minimum, int? maximum, string? pattern, string[]? enums)
        {
            CanBeNull = isNullable;
            _minimum = minimum;
            _maximum = maximum;
            _enums = enums;
            _pattern = pattern != null ? new Regex(pattern, RegexOptions.Compiled) : null;
        }

        public bool CanBeNull { get; }

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
