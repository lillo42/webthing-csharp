using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations.String
{
    /// <summary>
    /// Represent <see cref="DateTime"/> json schema validation.
    /// </summary>
    public class DateTimeJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;
        private readonly HashSet<DateTime>? _enums;
        
        /// <summary>
        /// Initialize a new instance of <see cref="DateTimeJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>=
        /// <param name="enums">The accepted values.</param>
        public DateTimeJsonSchemaValidation(bool isNullable, HashSet<DateTime>? enums)
        {
            _isNullable = isNullable;
            _enums = enums;
        }
        
        /// <inheritdoc/>
        public bool IsValid(object? value)
            => value switch
            {
                null => _isNullable,
                DateTime obj => _enums == null || _enums.Contains(obj),
                _ => false
            };
    }
}
