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
        private readonly HashSet<object>? _enum;

        /// <summary>
        /// Initialize a new instance of <see cref="ArrayJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>
        /// <param name="minItems">The minimum array length.</param>
        /// <param name="maxItems">The maximum array length.</param>
        /// <param name="uniqueItem">Accepted only unique items</param>
        /// <param name="enum">Accepted values.</param>
        public ArrayJsonSchemaValidation(bool isNullable, int? minItems, int? maxItems, bool uniqueItem, HashSet<object>? @enum)
        {
            _minItems = minItems;
            _maxItems = maxItems;
            _uniqueItem = uniqueItem;
            _enum = @enum;
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
                    if (_enum != null && !_enum.Contains(data))
                    {
                        return false;
                    }
                    
                    if (!hash.Add(data))
                    {
                        return false;
                    }
                }

                return true;
            }
            
            if (_enum != null)
            {
                foreach (var data in comparable)
                {
                    if (!_enum.Contains(data))
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
}
