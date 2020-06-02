namespace Mozilla.IoT.WebThing.Json.SchemaValidations
{
    /// <summary>
    /// Validate Json Schema.
    /// </summary>
    public interface IJsonSchemaValidation
    {
        /// <summary>
        /// Check if the object match with schema.
        /// </summary>
        /// <param name="value">The value to be compared</param>
        /// <returns>Return true if is valid, otherwise return false.</returns>
        bool IsValid(object? value);
    }
}
