using System;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations.String
{
    /// <summary>
    /// Represent <see cref="Enum"/> json schema validation.
    /// </summary>
    public class EnumJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;
        
        /// <summary>
        /// Initialize a new instance of <see cref="EnumJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>
        public EnumJsonSchemaValidation(bool isNullable)
        {
            _isNullable = isNullable;
        }
        
        /// <inheritdoc/>
        public bool IsValid(object? value)
            => value switch
            {
                null => _isNullable,
                _ => true
            };
    }
}
