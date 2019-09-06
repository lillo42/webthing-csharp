using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json
{
    public class JsonSchemaValidator : IJsonSchemaValidator
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
                var element = ((JsonElement)value);

                switch (type)
                {
                    case JsonType.Array:
                        return element.ValueKind == JsonValueKind.Array;
                    case JsonType.Boolean:
                        return element.ValueKind == JsonValueKind.False
                               || element.ValueKind == JsonValueKind.True;
                    case JsonType.Object:
                        return element.ValueKind == JsonValueKind.Object;
                    case JsonType.String:
                        return element.ValueKind == JsonValueKind.String;
                    case JsonType.Integer:
                    case JsonType.Number:
                        if (element.ValueKind == JsonValueKind.Number)
                        {
                            string text = element.GetRawText();
                            var number = GetNumber(text);

                            if (number != null)
                            {
                                validator.TryGetValue(MINIMUM, out var minimum);
                                var isValid = IsMinimumIsValid(number, minimum);
                                validator.TryGetValue(MAXIMUM, out var maximum);
                                isValid = isValid && IsMaximumIsValid(number, maximum);
                                validator.TryGetValue(MULTIPLE_OF, out var mutipleOf);
                                isValid = isValid && IsMultipleOf(number, mutipleOf);

                                return isValid;
                            }
                        }

                        return false;
                }
            }

            return true;
        }

        private static object GetNumber(string text)
        {
            if (short.TryParse(text, out var @short))
            {
                return @short;
            }

            else if (int.TryParse(text, out var @int))
            {
               return  @int;
            }

            else if (long.TryParse(text, out var @long))
            {
               return  @long;
            }

            else if (double.TryParse(text, out var @double))
            {
               return  @double;
            }

            else if (float.TryParse(text, out var @float))
            {
               return  @float;
            }

            else if (decimal.TryParse(text, out var @decimal))
            {
               return  @decimal;
            }

            else if (ushort.TryParse(text, out var @ushort))
            {
               return  @ushort;
            }

            else if (uint.TryParse(text, out var @uint))
            {
               return  @uint;
            }

            else if (ulong.TryParse(text, out var @ulong))
            {
               return  @ulong;
            }

            return null;
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
