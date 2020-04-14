using System;
using System.Linq;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Json.SchemaValidations;
using Mozilla.IoT.WebThing.Json.SchemaValidations.String;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc />
    public class SystemTexJsonSchemaValidationFactory : IJsonSchemaValidationFactory
    {
        /// <inheritdoc />
        public IJsonSchemaValidation Create(TypeCode typeCode, JsonSchema jsonSchema)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return new BooleanJsonSchemaValidation(jsonSchema.IsNullable);
                case TypeCode.String:
                    return new StringJsonSchemaValidation(jsonSchema.IsNullable, jsonSchema.MinimumLength,
                        jsonSchema.MaximumLength, jsonSchema.Pattern,
                        jsonSchema.Enums?.Where(x => x != null).Select(Convert.ToString).ToHashSet()!);
                case TypeCode.Char:
                    return new ChardJsonSchemaValidation(jsonSchema.IsNullable, 
                        jsonSchema.Enums?.Where(x => x != null).Select(Convert.ToChar).ToHashSet()!);
                case TypeCode.DateTime:
                    return new DateTimeJsonSchemaValidation(jsonSchema.IsNullable, 
                        jsonSchema.Enums?.Where(x => x != null).Select(Convert.ToDateTime).ToHashSet()!);
                case TypeCode.DateTimeOffset:
                    return new DateTimeOffsetJsonSchemaValidation(jsonSchema.IsNullable, 
                        jsonSchema.Enums?.Where(x => x != null).Select(x => DateTimeOffset.Parse(x.ToString()!)).ToHashSet()!);
                case TypeCode.Guid:
                    return new GuidJsonSchemaValidation(jsonSchema.IsNullable,
                        jsonSchema.Enums?.Where(x => x != null).Select(x => Guid.Parse(x.ToString()!)).ToHashSet()!);
                case TypeCode.TimeSpan:
                    return new TimeSpanJsonSchemaValidation(jsonSchema.IsNullable, 
                        jsonSchema.Enums?.Where(x => x != null).Select(x => TimeSpan.Parse(x.ToString()!)).ToHashSet()!);
                case TypeCode.Enum:
                    return new EnumJsonSchemaValidation(jsonSchema.IsNullable);
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Float:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return new NumberJsonSchemaValidation(jsonSchema.IsNullable, jsonSchema.Minimum,
                        jsonSchema.Maximum, jsonSchema.MultipleOf,
                        jsonSchema.Enums?.Where(x => x != null).Select(Convert.ToDecimal).ToHashSet()!);
                case TypeCode.Array:
                    return new ArrayJsonSchemaValidation(jsonSchema.IsNullable, 
                        jsonSchema.MinimumItems, jsonSchema.MaximumItems,
                        jsonSchema.UniqueItems, null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }
    }
}
