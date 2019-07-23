using System;
using System.Collections;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json
{
    public class DefaultJsonSchemaValidator : IJsonSchemaValidator
    {
        private const string IS_READ_ONLY = "readOnly";
        private const string TYPE = "type";
        private const string MINIMUM = "minimum";
        private const string MAXIMUM = "maximum";
        private const string MULTIPLE_OF = "multipleOf";
        public bool IsValid(object value, IDictionary<string, object> schema)
        {
            var validator = new Dictionary<string, object>(schema, StringComparer.OrdinalIgnoreCase);
            if (validator.ContainsKey(IS_READ_ONLY) && validator[IS_READ_ONLY] is bool isReadOnly && !isReadOnly)
            {
                return false;
            }

            if (validator.ContainsKey(TYPE) && Enum.TryParse<JsonType>(validator[TYPE].ToString(), true, out var type))
            {
                switch (type)
                {
                    case JsonType.Array:
                        return value is IEnumerable;
                    case JsonType.Boolean:
                        return value is bool;
                    case JsonType.Object:
                        return value.GetType().IsClass;
                    case JsonType.String:
                        return value is string;
                    case JsonType.Integer:
                    case JsonType.Number:
                        bool isValid = value is int
                                       || value is long
                                       || value is double
                                       || value is float
                                       || value is decimal
                                       || value is uint
                                       || value is ulong;
                        if (isValid)
                        {
                            validator.TryGetValue(MINIMUM, out object minimum);
                            isValid = IsMinimumIsValid(value, minimum);
                            validator.TryGetValue(MAXIMUM, out object maximum);
                            isValid = isValid && IsMaximumIsValid(value, maximum);
                            validator.TryGetValue(MULTIPLE_OF, out object mutipleOf);
                            isValid = isValid && IsMultipleOf(value, mutipleOf);
                        }
                        return isValid;
                }
            }
            return true;
        }

        private static bool IsMinimumIsValid(object value, object minimum)
        {
            if (minimum is null)
            {
                return true;
            }
            
            switch (value)
            {
                case int @int:
                    return @int > (int)minimum;
                case long @long:
                    return @long > (long)minimum;
                case double @double:
                    return @double > (double)minimum;
                case float @float:
                    return @float > (float)minimum;
                case decimal @decimal:
                    return @decimal > (decimal)minimum;
                case uint @uint:
                    return @uint > (uint)minimum;
                case ulong @ulong:
                    return @ulong > (ulong)minimum;
                default:
                    return false;
            }
        }
        
        private static bool IsMaximumIsValid(object value, object maximum)
        {
            if (maximum is null)
            {
                return true;
            }
            
            switch (value)
            {
                case int @int:
                    return @int < (int)maximum;
                case long @long:
                    return @long < (long)maximum;
                case double @double:
                    return @double < (double)maximum;
                case float @float:
                    return @float < (float)maximum;
                case decimal @decimal:
                    return @decimal < (decimal)maximum;
                case uint @uint:
                    return @uint < (uint)maximum;
                case ulong @ulong:
                    return @ulong < (ulong)maximum;
                default:
                    return false;
            }
        }
        
        private static bool IsMultipleOf(object value, object multipleOf)
        {
            if (multipleOf is null)
            {
                return true;
            }
            
            switch (value)
            {
                case int @int:
                    return @int % (int)multipleOf == 0;
                case long @long:
                    return @long % (long)multipleOf == 0;
                case double @double:
                    return @double % (double)multipleOf == 0;
                case float @float:
                    return @float % (float)multipleOf == 0;
                case decimal @decimal:
                    return @decimal % (decimal)multipleOf == 0;
                case uint @uint:
                    return @uint % (uint)multipleOf == 0;
                case ulong @ulong:
                    return @ulong % (ulong)multipleOf == 0;
                default:
                    return false;
            }
        }

        private enum JsonType
        {
            Array,
            Boolean,
            Integer,
            Number,
            Object,
            String
        }
    }
}

