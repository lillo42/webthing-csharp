using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Json.SchemaValidations;
using Mozilla.IoT.WebThing.Json.SchemaValidations.String;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc />
    public class SystemTexJsonSchemaValidationFactory : IJsonSchemaValidationFactory
    {
        /// <inheritdoc />
        public IJsonSchemaValidation Create(TypeCode typeCode, JsonSchema jsonSchema, Type type)
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
                        jsonSchema.Enums?.Where(x => x != null).Select(x => DateTimeOffset.Parse(x.ToString()!))
                            .ToHashSet()!);
                case TypeCode.Guid:
                    return new GuidJsonSchemaValidation(jsonSchema.IsNullable,
                        jsonSchema.Enums?.Where(x => x != null).Select(x => Guid.Parse(x.ToString()!)).ToHashSet()!);
                case TypeCode.TimeSpan:
                    return new TimeSpanJsonSchemaValidation(jsonSchema.IsNullable,
                        jsonSchema.Enums?.Where(x => x != null).Select(x => TimeSpan.Parse(x.ToString()!))
                            .ToHashSet()!);
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
                    var min = jsonSchema.Minimum;
                    var max = jsonSchema.Maximum;

                    if (jsonSchema.ExclusiveMinimum.HasValue)
                    {
                        min = jsonSchema.ExclusiveMinimum.Value + 1; 
                    }

                    if (jsonSchema.ExclusiveMaximum.HasValue)
                    {
                        max = jsonSchema.ExclusiveMaximum.Value - 1;
                    }

                    return new NumberJsonSchemaValidation(jsonSchema.IsNullable, min, max, 
                        jsonSchema.MultipleOf,
                        jsonSchema.Enums?.Where(x => x != null).Select(Convert.ToDecimal).ToHashSet());
                
                case TypeCode.Array:
                    HashSet<object>? hash = null;
                    
                    if(jsonSchema.Enums != null)
                    {
                        var arrayType = type.GetCollectionType().ToTypeCode();
                        switch (arrayType)
                        {
                            case TypeCode.String:
                                hash = jsonSchema.Enums.Where(x => x != null)
                                    .Select(Convert.ToString)
                                    .Cast<object>()
                                    .ToHashSet();
                                break;
                            case TypeCode.Char:
                                hash = jsonSchema.Enums.Where(x => x != null)
                                    .Select(Convert.ToChar)
                                    .Cast<object>()
                                    .ToHashSet();
                                break;
                            case TypeCode.DateTime:
                                hash = jsonSchema.Enums.Where(x => x != null)
                                    .Select(Convert.ToDateTime)
                                    .Cast<object>()
                                    .ToHashSet();
                                break;
                            case TypeCode.DateTimeOffset:
                                hash = jsonSchema.Enums.Where(x => x != null)
                                    .Select(x => DateTimeOffset.Parse(x.ToString()!))
                                    .Cast<object>()
                                    .ToHashSet();
                                break;
                            case TypeCode.Guid:
                                hash = jsonSchema.Enums.Where(x => x != null)
                                    .Select(x => Guid.Parse(x.ToString()!))
                                    .Cast<object>()
                                    .ToHashSet();
                                break;
                            case TypeCode.TimeSpan:
                                hash = jsonSchema.Enums.Where(x => x != null)
                                    .Select(x => TimeSpan.Parse(x.ToString()!))
                                    .Cast<object>()
                                    .ToHashSet();
                                break;
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
                                hash = jsonSchema.Enums.Where(x => x != null)
                                    .Select(Convert.ToDecimal)
                                    .Cast<object>()
                                    .ToHashSet();
                                break;
                        }
                    }
                    
                    return new ArrayJsonSchemaValidation(jsonSchema.IsNullable, 
                        jsonSchema.MinimumItems, jsonSchema.MaximumItems,
                        jsonSchema.UniqueItems.GetValueOrDefault(),
                        hash);
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }
    }
}
