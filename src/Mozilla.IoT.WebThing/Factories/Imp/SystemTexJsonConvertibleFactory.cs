using System;
using System.Reflection;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.Convertibles.Array;
using Mozilla.IoT.WebThing.Json.Convertibles.String;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc />
    public class SystemTexJsonConvertibleFactory : IJsonConvertibleFactory
    {
        /// <inheritdoc />
        public IJsonConvertible Create(TypeCode typeCode, Type type)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return SystemTexJsonBooleanConvertible.Instance;
                case TypeCode.String:
                    return SystemTexJsonStringConvertible.Instance;
                case TypeCode.Char:
                    return SystemTexJsonCharConvertible.Instance;
                case TypeCode.DateTime:
                    return SystemTexJsonDateTimeConvertible.Instance;
                case TypeCode.DateTimeOffset:
                    return SystemTexJsonDateTimeOffsetConvertible.Instance;
                case TypeCode.Guid:
                    return SystemTexJsonGuidConvertible.Instance;
                case TypeCode.TimeSpan:
                    return SystemTexJsonTimeSpanConvertible.Instance;
                case TypeCode.Enum:
                    return new SystemTexJsonEnumConvertible(@type);
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
                    return SystemTexJsonNumberConvertible.Instance;
                case TypeCode.Array:
                    var arrayType = type.GetCollectionType();
                    return new SystemTexJsonArrayConvertible(Create(arrayType.ToTypeCode(), arrayType));
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }
    }
}
