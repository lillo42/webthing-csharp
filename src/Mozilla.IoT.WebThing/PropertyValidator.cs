using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozilla.IoT.WebThing
{
    public class PropertyValidator : IPropertyValidator
    {
        private readonly bool _isReadOnly;
        private readonly object[]? _enums;
        private readonly float? _minimum;
        private readonly float? _maximum;
        private readonly float? _multipleOf;

        public PropertyValidator(bool isReadOnly, float? minimum, float? maximum, float? multipleOf, object[]? enums)
        {
            _isReadOnly = isReadOnly;
            _minimum = minimum;
            _maximum = maximum;
            _multipleOf = multipleOf;
            _enums = enums;
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
                || _multipleOf.HasValue)
            {
                var comparer = Convert.ToSingle(value);
                if (_minimum.HasValue && comparer < _minimum.Value)
                {
                    return false;
                }

                if (_maximum.HasValue && comparer > _maximum.Value)
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
                
                return x.Equals(value);
            }))
            {
                return false;
            }
            
            return true;
        }
    }
}
