using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations
{
    /// <summary>
    /// Represent array json schema validation.
    /// </summary>
    public class ArrayJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;
        private readonly int? _minItems;
        private readonly int? _maxItems;
        private readonly bool _uniqueItem;
        private readonly JsonType? _acceptedType;

        /// <summary>
        /// Initialize a new instance of <see cref="ArrayJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>
        /// <param name="minItems">The minimum array length.</param>
        /// <param name="maxItems">The maximum array length.</param>
        /// <param name="uniqueItem">Accepted only unique items</param>
        /// <param name="acceptedType">Accepted type.</param>
        public ArrayJsonSchemaValidation(bool isNullable, int? minItems, int? maxItems, bool uniqueItem, JsonType? acceptedType)
        {
            _minItems = minItems;
            _maxItems = maxItems;
            _uniqueItem = uniqueItem;
            _acceptedType = acceptedType;
            _isNullable = isNullable;
        }
        
        /// <inheritdoc/>
        public bool IsValid(object? value)
        {
            if (value == null)
            {
                return _isNullable;
            }
            
            if (!(value is object[] comparable))
            {
                return false;
            }

            if (_minItems.HasValue && comparable.Length < _minItems)
            {
                return false;
            }
            
            if (_maxItems.HasValue && comparable.Length > _maxItems)
            {
                return false;
            }

            if (_uniqueItem)
            {
                var hash = new HashSet<object?>();
                foreach (var data in comparable)
                {
                    if (data != null && _acceptedType != null)
                    {
                        switch (_acceptedType)
                        {
                            case JsonType.Boolean:
                                if (!(data is bool))
                                {
                                    return false;
                                }
                                break;
                            case JsonType.String:
                                if (!(data is string))
                                {
                                    return false;
                                }
                                break;
                            case JsonType.Integer:
                            case JsonType.Number:
                                if (!(data is decimal))
                                {
                                    return false;
                                }
                                break;
                            case null:
                            case JsonType.Array:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    if (!hash.Add(data))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (_acceptedType == null)
            {
                return true;
            }
            
            foreach (var data in comparable)
            {
                if (data != null && _acceptedType != null)
                {
                    switch (_acceptedType)
                    {
                        case JsonType.Boolean:
                            if (!(data is bool))
                            {
                                return false;
                            }
                            break;
                        case JsonType.String:
                            if (!(data is string))
                            {
                                return false;
                            }
                            break;
                        case JsonType.Integer:
                        case JsonType.Number:
                            if (!(data is decimal))
                            {
                                return false;
                            }
                            break;
                        case null:
                        case JsonType.Array:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return true;
        }
    }
}
