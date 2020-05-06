namespace Mozilla.IoT.WebThing.Json.SchemaValidations
{
    /// <summary>
    /// Represent boolean json schema validation.
    /// </summary>
    public class BooleanJsonSchemaValidation : IJsonSchemaValidation
    {
        private readonly bool _isNullable;

        /// <summary>
        /// Initialize a new instance of <see cref="BooleanJsonSchemaValidation"/>.
        /// </summary>
        /// <param name="isNullable">Accepted null value.</param>
        public BooleanJsonSchemaValidation(bool isNullable)
        {
            _isNullable = isNullable;
        }

        /// <inheritdoc/>
        public bool IsValid(object? value)
        {
            if (value == null)
            {
                return _isNullable;
            }

            return value is bool;
        }
    }
}
