using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations.String
{
    /// <summary>
    /// Represent <see cref="Guid"/> json schema validation.
    /// </summary>
    public class GuidJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;
        private readonly HashSet<Guid>? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="GuidJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>=
        /// <param name="enums">The accepted values.</param>
        public GuidJsonSchemaValidation(bool isNullable, HashSet<Guid>? enums)
        {
            _isNullable = isNullable;
            _enums = enums;
        }

        /// <inheritdoc/>
        public bool IsValid(object? value)
            => value switch
            {
                null => _isNullable,
                Guid obj => _enums == null || _enums.Contains(obj),
                _ => false
            };
    }
}
