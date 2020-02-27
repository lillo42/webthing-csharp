using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing
{
    public class PropertyValidator : IPropertyValidator
    {
        private enum JsonType
        {
            String,
            Number,
            Array,
            Bool
        }

        private readonly object[]? _enums;
        private readonly double? _minimum;
        private readonly double? _maximum;
        private readonly double? _exclusiveMinimum;
        private readonly double? _exclusiveMaximum;
        private readonly int? _multipleOf;
        private readonly uint? _minimumLength;
        private readonly uint? _maximumLength;
        private readonly Regex? _patter;
        private readonly JsonType _type;

        public PropertyValidator(bool isReadOnly,
            Type propertyType,
            ThingPropertyAttribute? propertyAttribute)
        {
            IsReadOnly = isReadOnly;

            if (propertyAttribute != null)
            {
                _enums = propertyAttribute.Enum;

                _minimum = propertyAttribute.MinimumValue;
                _maximum = propertyAttribute.MaximumValue;
                _multipleOf = propertyAttribute.MultipleOfValue;

                _exclusiveMinimum = propertyAttribute.ExclusiveMinimumValue;
                _exclusiveMaximum = propertyAttribute.ExclusiveMaximumValue;

                _minimumLength = propertyAttribute.MinimumLengthValue;
                _maximumLength = propertyAttribute.MaximumLengthValue;
                _patter = propertyAttribute.Pattern != null
                    ? new Regex(propertyAttribute.Pattern!, RegexOptions.Compiled)
                    : null;
            }

            if (propertyType == typeof(string))
            {
                _type = JsonType.String;
            }
            else if(propertyType == typeof(bool))
            {
                _type = JsonType.Bool;
            }
            else if(propertyType == typeof(byte)
                || propertyType == typeof(sbyte)
                || propertyType == typeof(short)
                || propertyType == typeof(ushort)
                || propertyType == typeof(int)
                || propertyType == typeof(uint)
                || propertyType == typeof(long)
                || propertyType == typeof(ulong)
                || propertyType == typeof(float)
                || propertyType == typeof(double)
                || propertyType == typeof(decimal))
            {
                _type = JsonType.Number;

                _enums = _enums?.Select(x =>
                {
                    if (x == null)
                    {
                        return (object)null;
                    }
                    return Convert.ToDouble(x);
                }).Distinct().ToArray();
            }
        }

        public bool IsReadOnly { get; }

        public bool IsValid(object? value)
        {
            if (IsReadOnly)
            {
                return false;
            }

            if (_type == JsonType.Number)
            {
                if (!IsValidNumber(value))
                {
                    return false;
                }
            }

            if (_type == JsonType.String)
            {
                if (!IsValidString(value))
                {
                    return false;
                }
            }

            return true;
        }


        private bool IsValidNumber(object value)
        {
            if (!_minimum.HasValue 
                && !_maximum.HasValue && !_multipleOf.HasValue 
                && !_exclusiveMinimum.HasValue 
                && !_exclusiveMaximum.HasValue
                && _enums == null)
            {
                return true;
            }

            var isNull = value == null;
            var comparer = Convert.ToDouble(value ?? 0);
            if (_minimum.HasValue && (isNull || comparer < _minimum.Value))
            {
                return false;
            }

            if (_maximum.HasValue && comparer > _maximum.Value)
            {
                return false;
            }

            if (_exclusiveMinimum.HasValue && (isNull || comparer <= _exclusiveMinimum.Value))
            {
                return false;
            }

            if (_exclusiveMaximum.HasValue && comparer >= _exclusiveMaximum.Value)
            {
                return false;
            }

            if (_multipleOf.HasValue && (isNull || comparer % _multipleOf.Value != 0))
            {
                return false;
            }

            if (_enums != null && !_enums.Any(x =>
            {
                if (isNull && x == null)
                {
                    return true;
                }
                
                return comparer.Equals(x);
            }))
            {
                return false;
            }
            
            return true;
        }

        private bool IsValidString(object value)
        {
            if (!_minimumLength.HasValue 
                && !_maximumLength.HasValue 
                && _patter == null
                && _enums == null)
            {
                return true;
            }

            var isNull = value == null;
            var comparer = Convert.ToString(value ?? string.Empty);
            if (_minimumLength.HasValue && (isNull || comparer.Length < _minimumLength.Value))
            {
                return false;
            }
            
            if (_maximumLength.HasValue && comparer.Length > _maximumLength.Value)
            {
                return false;
            }

            if (_patter != null && !_patter.Match(comparer).Success)
            {
                return false;
            }
            
            if (_enums != null && !_enums.Any(x =>
            {
                if (isNull && x == null)
                {
                    return true;
                }
                
                return comparer.Equals(x);
            }))
            {
                return false;
            }
            
            return true;
        }
    }
}
