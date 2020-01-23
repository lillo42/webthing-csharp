using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Factories.Generator.Converter
{
    internal class Utf8JsonWriterILGenerator
    {
        private readonly ILGenerator _ilGenerator;
        private readonly JsonSerializerOptions _options;

        public Utf8JsonWriterILGenerator(ILGenerator ilGenerator, JsonSerializerOptions options)
        {
            _ilGenerator = ilGenerator ?? throw new ArgumentNullException(nameof(ilGenerator));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        
        #region Types Functions 
        private readonly MethodInfo s_writeStartObjectWithName = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteStartObject), new [] { typeof(string)} );
        private readonly MethodInfo s_writeStartObject = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteStartObject), new Type[0]);
        private readonly MethodInfo s_writeEndObject = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteEndObject), new Type[0]);
        
        private readonly MethodInfo s_writeStartArray = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteStartArray), new [] { typeof(string)});
        private readonly MethodInfo s_writeEndArray = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteEndArray), new Type[0]);
        
        private readonly MethodInfo s_writeString = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteString), new []{ typeof(string), typeof(string) });
        private readonly MethodInfo s_writeStringValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteStringValue), new []{ typeof(string) });
        private readonly MethodInfo s_writeBool = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteBoolean), new []{ typeof(string), typeof(bool) });
        private readonly MethodInfo s_writeBoolValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteBooleanValue), new []{ typeof(bool) });
        private readonly MethodInfo s_writeNull = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNull), new []{ typeof(string) });
        private readonly MethodInfo s_writeNullValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNullValue), new Type[0]);
        
        private readonly MethodInfo s_writeNumberInt = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumber), new    [] { typeof(string), typeof(int) });
        private readonly MethodInfo s_writeNumberUInt = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumber), new   [] { typeof(string), typeof(uint) });
        private readonly MethodInfo s_writeNumberLong = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumber), new   [] { typeof(string), typeof(long) });
        private readonly MethodInfo s_writeNumberULong = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumber), new  [] { typeof(string), typeof(ulong) });
        private readonly MethodInfo s_writeNumberDouble = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumber), new [] { typeof(string), typeof(double) });
        private readonly MethodInfo s_writeNumberFloat = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumber), new  [] { typeof(string), typeof(float) });
        
        private readonly MethodInfo s_writeNumberIntValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumberValue), new []{ typeof(int) });
        private readonly MethodInfo s_writeNumberUIntValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumberValue), new []{ typeof(uint) });
        private readonly MethodInfo s_writeNumberLongValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumberValue), new []{ typeof(long) });
        private readonly MethodInfo s_writeNumberULongValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumberValue), new []{ typeof(ulong) });
        private readonly MethodInfo s_writeNumberDoubleValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumberValue), new []{ typeof(double) });
        private readonly MethodInfo s_writeNumberFloatValue = typeof(Utf8JsonWriter).GetMethod(nameof(Utf8JsonWriter.WriteNumberValue), new []{ typeof(float) });
        #endregion

        #region Object

        public void StartObject(string propertyName)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeStartObjectWithName, new[] { typeof(string) });
        }
        
        public void StartObject()
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeStartObject, null);
        }
        
        public void EndObject()
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeEndObject, null);
        }

        #endregion
        
        #region Array

        public void StartArray(string propertyName)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeStartArray, new[] { typeof(string) });
        }

        public void EndArray()
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeEndArray, null);
        }

        #endregion

        #region Properties

        public void PropertyWithNullValue(string propertyName)
        {
            if (!_options.IgnoreNullValues)
            {
                _ilGenerator.Emit(OpCodes.Ldarg_1);
                _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
                _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNull, new[] {typeof(string)});
            }
        }
        
        public void PropertyWithValue(string propertyName, string value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr,_options.PropertyNamingPolicy.ConvertName(propertyName));
            _ilGenerator.Emit(OpCodes.Ldstr, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeString, new[] { typeof(string), typeof(string) });
        }
        
        public void PropertyWithValue(string propertyName, bool value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.PropertyNamingPolicy.ConvertName(propertyName));
            _ilGenerator.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeBool, new[] { typeof(string), typeof(bool) });
        }

        #region Number

        public void PropertyWithValue(string propertyName, int value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
            _ilGenerator.Emit(OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberInt, new[] { typeof(string), typeof(int) });
        }
        
        public void PropertyWithValue(string propertyName, uint value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
            _ilGenerator.Emit(OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberUInt, new[] { typeof(string), typeof(uint) });
        }
        
        public void PropertyWithValue(string propertyName, long value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
            _ilGenerator.Emit(OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberLong, new[] { typeof(string), typeof(long) });
        }
        
        public void PropertyWithValue(string propertyName, ulong value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
            _ilGenerator.Emit(OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberULong, new[] { typeof(string), typeof(long) });
        }
        
        public void PropertyWithValue(string propertyName, double value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
            _ilGenerator.Emit(OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberDouble, new[] { typeof(string), typeof(double) });
        }
        
        public void PropertyWithValue(string propertyName, float value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, _options.GetPropertyName(propertyName));
            _ilGenerator.Emit(OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberFloat, new[] { typeof(string), typeof(float) });
        }

        #endregion

        #endregion
        
        #region Properties With Nullable Valeues

        public void PropertyWithNullableValue(string propertyName, string? value)
        {
            if (value == null)
            {
                PropertyWithNullValue(propertyName);
            }
            else
            {
                PropertyWithValue(propertyName, value);
            }
        }
        
        public void PropertyWithNullableValue(string propertyName, bool? value)
        {
            if (value.HasValue)
            {
                PropertyWithValue(propertyName, value.Value);
            }
            else
            {
                PropertyWithNullValue(propertyName);
            }
        }
        
        public void PropertyWithNullableValue(string propertyName, int? value)
        {
            if (value.HasValue)
            {
                PropertyWithValue(propertyName, value.Value);
            }
            else
            {
                PropertyWithNullValue(propertyName);
            }
        }

        #endregion

        #region Value

        public void NullValue()
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNullValue, null);
        }
        
        public void Value(string value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(OpCodes.Ldstr, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeStringValue, new[] { typeof(string) });
        }
        
        public void Value(bool value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit(value ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeBoolValue, new[] { typeof(bool) });
        }
        
        public void Value(int value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit( OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberIntValue, new[] { typeof(int) });
        }
        
        public void Value(uint value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit( OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberUIntValue, new[] { typeof(uint) });
        }
        
        
        public void Value(long value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit( OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberLongValue, new[] { typeof(long) });
        }
        
        public void Value(ulong value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit( OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberULongValue, new[] { typeof(ulong) });
        }
        
        public void Value(double value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit( OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberDoubleValue, new[] { typeof(double) });
        }
        
        public void Value(float value)
        {
            _ilGenerator.Emit(OpCodes.Ldarg_1);
            _ilGenerator.Emit( OpCodes.Ldc_I4_S, value);
            _ilGenerator.EmitCall(OpCodes.Callvirt, s_writeNumberFloatValue, new[] { typeof(float) });
        }

        #endregion
        
        public void PropertyType(string propertyName, string[]? types)
        {
            if (types == null)
            {
                PropertyWithNullValue(propertyName);
            }
            else if (types.Length == 1)
            {
                PropertyWithValue(propertyName, types[0]);
            }
            else
            {
                StartArray(propertyName);

                foreach (var value in types)
                {
                    if (value == null)
                    {
                        NullValue();
                    }
                    else
                    {
                        Value(value);
                    }
                }
                    
                EndArray();
            }
        }
        
        public void PropertyNumber(string propertyName, Type propertyType, float? value)
            {
                if (value == null)
                {
                    PropertyWithNullValue(propertyName);
                    return;
                }

                if (propertyType == typeof(int)
                    || propertyType == typeof(byte)
                    || propertyType == typeof(short)
                    || propertyType == typeof(ushort))
                {
                    PropertyWithValue(propertyName, (int)value);
                }
                else if (propertyType == typeof(uint))
                {
                    PropertyWithValue(propertyName, (uint)value);
                }
                else if (propertyType == typeof(long))
                {
                    PropertyWithValue(propertyName, (long)value);
                }
                else if (propertyType == typeof(ulong))
                {
                    PropertyWithValue(propertyName, (ulong)value);
                }
                else if (propertyType == typeof(double))
                {
                    PropertyWithValue(propertyName, (double)value);
                }
                else
                {
                    PropertyWithValue(propertyName, (float)value);
                }
            }

        public void PropertyEnum(string propertyName, Type propertyType, object[]? @enums)
        {
            if (enums == null)
            {
                PropertyWithNullValue(propertyName);
                return;
            }
            
            StartArray(propertyName);

            if (propertyType == typeof(string))
            {
                foreach (var @enum in enums)
                {
                    if (@enum == null)
                    {
                        NullValue();
                    }
                    else
                    {
                        Value((string)@enum);
                    }
                }
            }
            else if (propertyType == typeof(bool))
            {
                foreach (var @enum in enums)
                {
                    if (@enum == null)
                    {
                        NullValue();
                    }
                    else
                    {
                        Value((bool)@enum);
                    }
                }
            }
            else if (propertyType == typeof(int)
                     || propertyType == typeof(byte)
                     || propertyType == typeof(short)
                     || propertyType == typeof(ushort))
            {
                foreach (var @enum in enums)
                {
                    if (@enum == null)
                    {
                        NullValue();
                    }
                    else
                    {
                        Value((int)@enum);
                    }
                }
            }
            else if (propertyType == typeof(uint))
            {
                foreach (var @enum in enums)
                {
                    if (@enum == null)
                    {
                        NullValue();
                    }
                    else
                    {
                        Value((uint)@enum);
                    }
                }
            }
            else if (propertyType == typeof(long))
            {
                foreach (var @enum in enums)
                {
                    if (@enum == null)
                    {
                        NullValue();
                    }
                    else
                    {
                        Value((long)@enum);
                    }
                }
            }
            else if (propertyType == typeof(ulong))
            {
                foreach (var @enum in enums)
                {
                    if (@enum == null)
                    {
                        NullValue();
                    }
                    else
                    {
                        Value((ulong)@enum);
                    }
                }
            }
            else if (propertyType == typeof(double))
            {
                foreach (var @enum in enums)
                {
                    if (@enum == null)
                    {
                        NullValue();
                    }
                    else
                    {
                        Value((double)@enum);
                    }
                }
            }
            else if (propertyType == typeof(float))
            {
                foreach (var @enum in enums)
                {
                    if (@enum == null)
                    {
                        NullValue();
                    }
                    else
                    {
                        Value((float)@enum);
                    }
                }
            }
            
            EndArray();
        }
    }
}
