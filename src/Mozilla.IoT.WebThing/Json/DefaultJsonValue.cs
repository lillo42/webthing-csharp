using System;
using System.Text.Json;

using static System.Activator; 

namespace Mozilla.IoT.WebThing.Json
{
    public class DefaultJsonValue : IJsonValue
    {
        public object GetValue(object value, Type type)
        {
            if (value is JsonElement element)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Undefined:
                        return type.IsValueType ? CreateInstance(type) : null;
                    case JsonValueKind.Object:
                        break;
                    case JsonValueKind.Array:
                        break;
                    case JsonValueKind.String:
                        return element.GetString();
                    case JsonValueKind.Number:
                        return GetNumber(type, element);
                    case JsonValueKind.True:
                        return true;
                    case JsonValueKind.False:
                        return true;
                    case JsonValueKind.Null:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return value;
        }

        private static object GetNumber(Type type, JsonElement element)
        {
            if (type == typeof(short))
            {
                return element.GetInt16();
            }

            if (type == typeof(int))
            {
                return element.GetInt32();
            }

            if (type == typeof(long))
            {
                return element.GetInt64();
            }

            if (type == typeof(double))
            {
                return element.GetDouble();
            }
            
            if (type == typeof(float))
            {
                return element.GetSingle();
            }
            
            if (type == typeof(decimal))
            {
                return element.GetDecimal();
            }
            
            if (type == typeof(ushort))
            {
                return element.GetUInt16();
            }

            if (type == typeof(uint))
            {
                return element.GetUInt32();
            }

            if (type == typeof(ulong))
            {
                return element.GetUInt64();
            }

            return CreateInstance(type);
        }
        
        
    }
}
