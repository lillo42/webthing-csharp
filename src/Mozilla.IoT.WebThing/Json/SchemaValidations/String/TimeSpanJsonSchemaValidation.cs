using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations.String
{
    /// <summary>
    /// Represent <see cref="TimeSpan"/> json schema validation.
    /// </summary>
    public class TimeSpanJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;
        private readonly HashSet<TimeSpan>? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="TimeSpanJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>=
        /// <param name="enums">The accepted values.</param>
        public TimeSpanJsonSchemaValidation(bool isNullable, HashSet<TimeSpan>? enums)
        {
            _isNullable = isNullable;
            _enums = enums;
        }

        /// <inheritdoc/>
        public bool IsValid(object? value) 
            => value switch
            {
                null => _isNullable,
                TimeSpan obj => _enums == null || _enums.Contains(obj),
                _ => false
            };
    }
}
