using System;
using System.Reflection;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Array;
using Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Boolean;
using Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Number;
using Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Strings;
using TypeCode = Mozilla.IoT.WebThing.Factories.TypeCode;

namespace Mozilla.IoT.WebThing.Newtonsoft.Factories
{
    /// <summary>
    /// Newtonsoft implementation of <see cref="IJsonConvertibleFactory"/>
    /// </summary>
    public class NewtonsoftJsonConvertibleFactory : IJsonConvertibleFactory
    {
        /// <inheritdoc />
        public IJsonConvertible Create(TypeCode typeCode, Type type)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return NewtonsoftBooleanConvertible.Instance;
                case TypeCode.String:
                    return NewtonsoftStringConvertible.Instance;
                case TypeCode.Char:
                    return NewtonsoftCharConvertible.Instance;
                case TypeCode.DateTime:
                    return NewtonsoftDateTimeConvertible.Instance;
                case TypeCode.DateTimeOffset:
                    return NewtonsoftDateTimeOffsetConvertible.Instance;
                case TypeCode.Guid:
                    return NewtonsoftGuidConvertible.Instance;
                case TypeCode.TimeSpan:
                    return NewtonsoftTimeSpanConvertible.Instance;
                case TypeCode.Enum:
                    return new NewtonsoftEnumConvertible(type);
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
                    return NewtonsoftNumberConvertible.Instance;
                case TypeCode.Array:
                    var arrayType = type.GetCollectionType();
                    return new NewtonsoftArrayConvertible(Create(arrayType.ToTypeCode(), arrayType));
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }
    }
}
