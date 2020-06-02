using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations.String
{
    /// <summary>
    /// Represent <see cref="DateTimeOffset"/> json schema validation.
    /// </summary>
    public class DateTimeOffsetJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;
        private readonly HashSet<DateTimeOffset>? _enums;
        
        /// <summary>
        /// Initialize a new instance of <see cref="DateTimeOffsetJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>=
        /// <param name="enums">The accepted values.</param>
        public DateTimeOffsetJsonSchemaValidation(bool isNullable, HashSet<DateTimeOffset>? enums)
        {
            _isNullable = isNullable;
            _enums = enums;
        }
        
        /// <inheritdoc/>
        public bool IsValid(object? value)
            => value switch
            {
                null => _isNullable,
                DateTimeOffset obj => _enums == null || _enums.Contains(obj),
                _ => false
            };
    }
}
