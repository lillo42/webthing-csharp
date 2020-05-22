using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations.String
{
    /// <summary>
    /// Represent string json schema validation.
    /// </summary>
    public class StringJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;
        
        private readonly int? _minimum;
        private readonly int? _maximum;
        private readonly HashSet<string>? _enums;
        
        private readonly string? _pattern;

        /// <summary>
        /// Initialize a new instance of <see cref="StringJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>
        /// <param name="minimum">The minimum string length.</param>
        /// <param name="maximum">The maximum string length.</param>
        /// <param name="enums">The accepted values.</param>
        /// <param name="pattern">The string patterns.</param>
        public StringJsonSchemaValidation(bool isNullable, int? minimum, int? maximum, 
            string? pattern, HashSet<string>? enums)
        {
            _isNullable = isNullable;
            _minimum = minimum;
            _maximum = maximum;
            _enums = enums;
            _pattern = pattern;
        }

        /// <inheritdoc/>
        public bool IsValid(object? value)
        {
            if (value == null)
            {
                return _isNullable;
            }

            var comparable = value.ToString()!;
            
            if (_minimum.HasValue && comparable.Length < _minimum.Value)
            {
                return false;
            }
            
            if (_maximum.HasValue && comparable.Length > _maximum.Value)
            {
                return false;
            }
            
            if (!string.IsNullOrEmpty(_pattern) && !Regex.IsMatch(comparable, _pattern))
            {
                return false;
            }
            
            return _enums == null || _enums.Contains(comparable);
        }
    }
}
