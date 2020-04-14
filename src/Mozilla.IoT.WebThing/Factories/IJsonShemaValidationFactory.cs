using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Json.SchemaValidations;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <summary>
    /// The factory of <see cref="IJsonSchemaValidation"/>.
    /// </summary>
    public interface IJsonSchemaValidationFactory
    {
        /// <summary>
        /// Create new instance of <see cref="IJsonSchemaValidation"/> for <see cref="bool"/>.
        /// </summary>
        /// <param name="typeCode"></param>
        /// <param name="jsonSchema">The <see cref="JsonSchema"/>.</param>
        /// <returns>New instance of <see cref="IJsonSchemaValidation"/>.</returns>
        IJsonSchemaValidation Create(TypeCode typeCode, JsonSchema jsonSchema);
    }
}
