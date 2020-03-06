using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    public class JsonElementMethods
    {
        private readonly ILGenerator _generator;

        public JsonElementMethods(ILGenerator generator)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        private static readonly MethodInfo s_getTryGetByte = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetByte));
        private static readonly MethodInfo s_getTryGetSByte = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetSByte));
        private static readonly MethodInfo s_getTryGetShort = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetInt16));
        private static readonly MethodInfo s_getTryGetUShort = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetUInt16));
        private static readonly MethodInfo s_getTryGetInt = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetInt32));
        private static readonly MethodInfo s_getTryGetUInt = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetUInt32));
        private static readonly MethodInfo s_getTryGetLong = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetInt64));
        private static readonly MethodInfo s_getTryGetULong = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetUInt64));
        private static readonly MethodInfo s_getTryGetFloat = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetSingle));
        private static readonly MethodInfo s_getTryGetDouble = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetDouble));
        private static readonly MethodInfo s_getTryGetDecimal = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetDecimal));
        private static readonly MethodInfo s_getTryGetDateTime = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetDateTime));
        private static readonly MethodInfo s_getTryGetDateTimeOffset = typeof(JsonElement).GetMethod(nameof(JsonElement.TryGetDateTimeOffset));
        
        private static readonly MethodInfo s_getValueKind = typeof(JsonElement).GetProperty(nameof(JsonElement.ValueKind)).GetMethod;
        private static readonly MethodInfo s_getString = typeof(JsonElement).GetMethod(nameof(JsonElement.GetString));
        private static readonly MethodInfo s_getBool = typeof(JsonElement).GetMethod(nameof(JsonElement.GetBoolean));

        public static MethodInfo ValueKind => s_getValueKind;
        
        public void TryGet(Type type)
        {
            if (type == typeof(byte))
            {
                Call(s_getTryGetByte);
            }
            else if (type == typeof(sbyte))
            {
                Call(s_getTryGetSByte);
            }
            else if (type == typeof(short))
            {
                Call(s_getTryGetShort);
            }
            else if (type == typeof(ushort))
            {
                Call(s_getTryGetUShort);
            }
            else if (type == typeof(int))
            {
                Call(s_getTryGetInt);
            }
            else if (type == typeof(uint))
            {
                Call(s_getTryGetUInt);
            }
            else if (type == typeof(long))
            {
                Call(s_getTryGetLong);
            }
            else if (type == typeof(ulong))
            {
                Call(s_getTryGetULong);
            }
            else if (type == typeof(float))
            {
                Call(s_getTryGetFloat);
            }
            else if (type == typeof(double))
            {
                Call(s_getTryGetDouble);
            }
            else if (type == typeof(decimal))
            {
                Call(s_getTryGetDecimal);
            }
            else if (type == typeof(DateTime))
            {
                Call(s_getTryGetDateTime);
            }
            else if (type == typeof(DateTimeOffset))
            {
                Call(s_getTryGetDateTimeOffset);
            }
        }

        public static MethodInfo TryGetValue(Type type)
        {
            if (type == typeof(byte))
            {
                return s_getTryGetByte;
            }

            if (type == typeof(sbyte))
            {
                return s_getTryGetSByte;
            }
            if (type == typeof(short))
            {
                return s_getTryGetShort;
            }
            
            if (type == typeof(ushort))
            {
                return s_getTryGetUShort;
            }
            
            if (type == typeof(int))
            {
                return s_getTryGetInt;
            }
            
            if (type == typeof(uint))
            {
                return s_getTryGetUInt;
            }
            
            if (type == typeof(long))
            {
                return s_getTryGetLong;
            }
            
            if (type == typeof(ulong))
            {
                return s_getTryGetULong;
            }
            
            if (type == typeof(float))
            {
                return s_getTryGetFloat;
            }
            
            if (type == typeof(double))
            {
                return s_getTryGetDouble;
            }
            
            if (type == typeof(decimal))
            {
                return s_getTryGetDecimal;
            }
            
            if (type == typeof(DateTime))
            {
                return s_getTryGetDateTime;
            }
            
            if (type == typeof(DateTimeOffset))
            {
                return s_getTryGetDateTimeOffset;
            }

            return null;
        }
        
        public static MethodInfo GetValue(Type type)
        {
            if (type == typeof(string))
            {
                return s_getString;
            }
            
            if (type == typeof(bool))
            {
                return s_getBool;
            }

            return null;
        }
        
        private void Call(MethodInfo tryGet) 
            => _generator.EmitCall(OpCodes.Call, tryGet, null);

        public void GetValueKind() 
            => Call(s_getValueKind);
    }
}
