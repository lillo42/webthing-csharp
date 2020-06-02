using System;
using System.Reflection;
using Mozilla.IoT.WebThing.Builders;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc />
    public class PropertyFactory : IPropertyFactory
    {
        private readonly IJsonSchemaValidationFactory _validationFactory;
        private readonly IJsonConvertibleFactory _jsonConvertibleFactory;
        private readonly IConvertibleFactory _convertibleFactory;

        /// <summary>
        /// Initialize a new instance of <see cref="PropertyFactory"/>.
        /// </summary>
        /// <param name="validationFactory"></param>
        /// <param name="jsonConvertibleFactory"></param>
        /// <param name="convertibleFactory"></param>
        public PropertyFactory(IJsonSchemaValidationFactory validationFactory, 
            IJsonConvertibleFactory jsonConvertibleFactory, 
            IConvertibleFactory convertibleFactory)
        {
            _validationFactory = validationFactory ?? throw new ArgumentNullException(nameof(validationFactory));
            _jsonConvertibleFactory = jsonConvertibleFactory ?? throw new ArgumentNullException(nameof(jsonConvertibleFactory));
            _convertibleFactory = convertibleFactory ?? throw new ArgumentNullException(nameof(convertibleFactory));
        }

        /// <inheritdoc />
        public IThingProperty Create(Type propertyType, JsonSchema jsonSchema, Thing thing,
            Action<object, object?>? setter, Func<object, object?> getter, string originPropertyName)
        {
            propertyType = propertyType.GetUnderlyingType();
            var code = propertyType.ToTypeCode();

            var jsonConvertible = _jsonConvertibleFactory.Create(code, propertyType);
            var validation = _validationFactory.Create(code, jsonSchema, propertyType);
            
            switch (code)
            {
                case TypeCode.Boolean:
                case TypeCode.String:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.DateTimeOffset:
                case TypeCode.Guid:
                case TypeCode.TimeSpan:
                case TypeCode.Enum:
                    return new ThingProperty(thing, jsonSchema.IsReadOnly.GetValueOrDefault(), jsonSchema.IsWriteOnly.GetValueOrDefault(), 
                        getter, setter, validation, jsonConvertible, null, originPropertyName);
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
                    return new ThingProperty(thing, jsonSchema.IsReadOnly.GetValueOrDefault(), jsonSchema.IsWriteOnly.GetValueOrDefault(), 
                        getter, setter, validation, jsonConvertible,
                        _convertibleFactory.Create(code, propertyType), originPropertyName);
                case TypeCode.Array:
                    return new ThingProperty(thing, jsonSchema.IsReadOnly.GetValueOrDefault(), jsonSchema.IsWriteOnly.GetValueOrDefault(), getter, setter,
                        validation, jsonConvertible, _convertibleFactory.Create(code, propertyType), originPropertyName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
