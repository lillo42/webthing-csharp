using System.Linq;

namespace Mozilla.IoT.WebThing
{
    public class PropertyValidator : IPropertyValidator
    {
        private readonly bool _isReadOnly;
        private readonly object[]? _enums;
        private readonly float? _minimum;
        private readonly float? _maximum;
        private readonly int? _multipleOf;

        public PropertyValidator(bool isReadOnly, float? minimum, float? maximum, int? multipleOf, object[]? enums)
        {
            _isReadOnly = isReadOnly;
            _minimum = minimum;
            _maximum = maximum;
            _multipleOf = multipleOf;
            _enums = enums;
        }

        public bool IsValid(object? value)
        {
            if (_isReadOnly)
            {
                return false;
            }

            if (_minimum.HasValue && (float)value < _minimum)
            {
                return false;
            }

            if (_maximum.HasValue && (float)value > _maximum)
            {
                return false;
            }

            if (_multipleOf.HasValue && (float)value % _multipleOf != 1)
            {
                return false;
            }

            if (_enums != null && _enums.All(x => !x.Equals(value)))
            {
                return false;
            }
            
            return true;
        }
    }
}
