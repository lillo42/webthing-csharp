using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json.SchemaValidations.String
{
    /// <summary>
    /// Represent <see cref="char"/> json schema validation.
    /// </summary>
    public class ChardJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;
        private readonly HashSet<char>? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="ChardJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>=
        /// <param name="enums">The accepted values.</param>
        public ChardJsonSchemaValidation(bool isNullable, HashSet<char>? enums)
        {
            _isNullable = isNullable;
            _enums = enums;
        }

        /// <inheritdoc/>
        public bool IsValid(object? value)
            => value switch
            {
                null => _isNullable,
                char obj => _enums == null || _enums.Contains(obj),
                _ => false
            };
    }
}
