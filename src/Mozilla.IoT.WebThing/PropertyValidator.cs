using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozilla.IoT.WebThing
{
    public class PropertyValidator : IPropertyValidator
    {
        private readonly bool _isReadOnly;
        private readonly object[]? _enums;
        private readonly double? _minimum;
        private readonly double? _maximum;
        private readonly double? _exclusiveMinimum;
        private readonly double? _exclusiveMaximum;
        private readonly int? _multipleOf;
        private readonly bool _acceptedNullableValue;

        public PropertyValidator(bool isReadOnly, 
            double? minimum, double? maximum, int? multipleOf, 
            object[]? enums, bool acceptedNullableValue, 
            double? exclusiveMinimum, double? exclusiveMaximum)
        {
            _isReadOnly = isReadOnly;
            _minimum = minimum;
            _maximum = maximum;
            _multipleOf = multipleOf;
            _enums = enums;
            _acceptedNullableValue = acceptedNullableValue;
            _exclusiveMinimum = exclusiveMinimum;
            _exclusiveMaximum = exclusiveMaximum;
        }

        public bool IsReadOnly => _isReadOnly;

        public bool IsValid(object? value)
        {
            if (_isReadOnly)
            {
                return false;
            }

            if (_minimum.HasValue
                || _maximum.HasValue
                || _multipleOf.HasValue
                || _exclusiveMinimum.HasValue
                || _exclusiveMaximum.HasValue)
            {

                if (_acceptedNullableValue && value == null)
                {
                    return true;
                }
                
                var comparer = Convert.ToDouble(value);
                if (_minimum.HasValue && comparer < _minimum.Value)
                {
                    return false;
                }

                if (_maximum.HasValue && comparer > _maximum.Value)
                {
                    return false;
                }
                
                if (_exclusiveMinimum.HasValue && comparer <= _exclusiveMinimum.Value)
                {
                    return false;
                }

                if (_exclusiveMaximum.HasValue && comparer >= _exclusiveMaximum.Value)
                {
                    return false;
                }

                if (_multipleOf.HasValue && comparer % _multipleOf.Value != 0)
                {
                    return false;
                }
            }

            if (_enums != null && !_enums.Any(x =>
            {
                if (value == null && x == null)
                {
                    return true;
                }
                
                return value.Equals(x);
            }))
            {
                return false;
            }

            if (!_acceptedNullableValue && value == null)
            {
                return false;
            }
            
            return true;
        }
    }
}
