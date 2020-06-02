using System.Collections;
using System.Linq;
using Mozilla.IoT.WebThing.Json;
using Code = Mozilla.IoT.WebThing.Factories.TypeCode;

namespace System.Reflection
{
    /// <summary>
    /// Extensions of <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// If type is nullable
        /// </summary>
        /// <param name="type">The type to be check</param>
        /// <returns>True if type is nullable otherwise is false</returns>
        public static bool IsNullable(this Type type)
            => type.IsClass
               || type.IsArray 
               || type.IsInterface
               || Nullable.GetUnderlyingType(type) != null;

        /// <summary>
        /// Get type if is null.
        /// </summary>
        /// <param name="type">The type to get</param>
        /// <returns>Return type.</returns>
        public static Type GetUnderlyingType(this Type type)
            => Nullable.GetUnderlyingType(type) ?? type;
        
        /// <summary>
        /// Get Collection type
        /// </summary>
        /// <param name="type">The type to get</param>
        /// <returns>Return type.</returns>
        public static Type GetCollectionType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType()!;
            }

            if (type.IsGenericType)
            {
                return type.GetGenericArguments()[0];
            }

            return typeof(object);
        }
        
        /// <summary>
        /// Convert <see cref="Type"/> to <see cref="JsonType"/>.
        /// </summary>
        /// <param name="type">The type to get</param>
        /// <returns>Return <see cref="JsonType"/>.</returns>
        public static JsonType ToJsonType(this Type type)
        {
            type = type.GetUnderlyingType();
             
            if (type == typeof(string)
                || type == typeof(char)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(Guid)
                || type == typeof(TimeSpan)
                || type.IsEnum)
            {
                return JsonType.String;
            }

            if (type == typeof(bool))
            {
                return JsonType.Boolean;
            }
            
            if (type == typeof(int)
                || type == typeof(sbyte)
                || type == typeof(byte)
                || type == typeof(short)
                || type == typeof(long)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(ushort))
            {
                return JsonType.Integer;
            }
            
            if (type == typeof(double)
                || type == typeof(float)
                || type == typeof(decimal))
            {
                return JsonType.Number;
            }

            if (type.IsArray
                || type.GetInterfaces().Any(x => x == typeof(IEnumerable)))
            {
                return JsonType.Array;
            }

            return JsonType.Object;
        }

        /// <summary>
        /// Convert <see cref="Type"/> to <see cref="Code"/>.
        /// </summary>
        /// <param name="type">The type to get</param>
        /// <returns>Return <see cref="Code"/>.</returns>
        public static Code ToTypeCode(this Type type)
        {
            type = type.GetUnderlyingType();

            if (type == typeof(bool))
            {
                return Code.Boolean;
            }

            #region String

            if (type.IsEnum)
            {
                return Code.Enum;
            }

            if (type == typeof(string))
            {
                return Code.String;
            }

            if (type == typeof(char))
            {
                return Code.Char;
            }

            if (type == typeof(DateTime))
            {
                return Code.DateTime;
            }

            if (type == typeof(DateTimeOffset))
            {
                return Code.DateTimeOffset;
            }

            if (type == typeof(Guid))
            {
                return Code.Guid;
            }

            if (type == typeof(TimeSpan))
            {
                return Code.TimeSpan;
            }

            #endregion

            #region Number

            if (type == typeof(int))
            {
                return Code.Int32;
            }

            if (type == typeof(sbyte))
            {
                return Code.SByte;
            }

            if (type == typeof(byte))
            {
                return Code.Byte;
            }

            if (type == typeof(short))
            {
                return Code.Int16;
            }

            if (type == typeof(long))
            {
                return Code.Int64;
            }

            if (type == typeof(ushort))
            {
                return Code.UInt16;
            }

            if (type == typeof(uint))
            {
                return Code.UInt32;
            }

            if (type == typeof(ulong))
            {
                return Code.UInt64;
            }

            if (type == typeof(float))
            {
                return Code.Float;
            }

            if (type == typeof(double))
            {
                return Code.Double;
            }

            if (type == typeof(decimal))
            {
                return Code.Decimal;
            }

            #endregion

            if (type.IsArray
                || type.GetInterfaces().Any(x => x == typeof(IEnumerable))
                || type == typeof(IEnumerable))
            {
                return Code.Array;
            }
            
            return Code.Object;
        }
    }
}
