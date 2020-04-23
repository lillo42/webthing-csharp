using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations
{
    /// <summary>
    ///  Represent number json schema validation.
    /// </summary>
    public class NumberJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;
        private readonly decimal? _minimum;
        private readonly decimal? _maximum;
        private readonly decimal? _multipleOf;
        private readonly HashSet<decimal>? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="NumberJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <param name="multipleOf">The multiple of value</param>
        /// <param name="enums">The accepted values.</param>
        public NumberJsonSchemaValidation(bool isNullable, decimal? minimum, decimal? maximum, decimal? multipleOf, 
            HashSet<decimal>? enums)
        {
            _isNullable = isNullable;
            _minimum = minimum;
            _maximum = maximum;
            _multipleOf = multipleOf;
            _enums = enums;
        }
        
        /// <inheritdoc/>
        public bool IsValid(object? value)
        {
            if (value == null)
            {
                return _isNullable;
            }

            if (!(value is decimal comparable))
            {
                return false;
            }

            if (_minimum.HasValue && comparable.CompareTo(_minimum.Value) == -1)
            {
                return false;
            }
            
            if (_maximum.HasValue && comparable.CompareTo(_maximum.Value) == 1)
            {
                return false;
            }

            if (_multipleOf.HasValue && comparable % _multipleOf.Value != 0)
            {
                return false;
            }

            if (_enums != null && !_enums.Contains(comparable))
            {
                return false;
            }

            return true;
        }    
    }
}
