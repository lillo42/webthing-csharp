using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    public class ValidationGeneration
    {
        private readonly ILGenerator _generator;
        private readonly TypeBuilder _builder;
        
        private static readonly MethodInfo s_getLength = typeof(string).GetProperty(nameof(string.Length)).GetMethod;
        private static readonly MethodInfo s_match = typeof(Regex).GetMethod(nameof(Regex.Match) , new [] { typeof(string) });
        private static readonly MethodInfo s_success = typeof(Match).GetProperty(nameof(Match.Success)).GetMethod;
        private static readonly ConstructorInfo s_regexConstructor = typeof(Regex).GetConstructors()[1];
        
        private static readonly MethodInfo s_toDecimal = typeof(Convert).GetMethod(nameof(Convert.ToDecimal), new[] {typeof(string)});
        private static readonly MethodInfo s_decimalComparer = typeof(decimal).GetMethod(nameof(decimal.Compare), new[] {typeof(decimal), typeof(decimal)});
        private static readonly MethodInfo s_decimalRemainder = typeof(decimal).GetMethod(nameof(decimal.Remainder), new[] {typeof(decimal), typeof(decimal)});
        private static readonly FieldInfo s_decimalZero = typeof(decimal).GetField(nameof(decimal.Zero));
        
        public ValidationGeneration(ILGenerator generator, TypeBuilder builder)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _builder = builder;
        }
        
        public void AddValidation(Type type, Validation validation, Action getValue, Action error, ref Label? next)
        {
            if (IsNumber(type))
            {
                AddNumberValidation(type, validation, getValue, error, ref next);
            }

            if (IsString(type))
            {
                AddStringValidation(validation, getValue, error, ref next);
            }
        }
        
        private void AddNumberValidation(Type type, Validation validation, Action getValue, Action error, ref Label? next)
        {
            if (type == typeof(decimal))
            {
                if (validation.Minimum.HasValue)
                {
                    GenerateDecimalValidation(OpCodes.Bge_S, validation.Minimum.Value, ref next);
                }
                
                if (validation.Maximum.HasValue)
                {
                    GenerateDecimalValidation(OpCodes.Ble_S, validation.Maximum.Value, ref next);
                }
                
                if (validation.ExclusiveMinimum.HasValue)
                {
                    GenerateDecimalValidation(OpCodes.Bgt_S, validation.ExclusiveMinimum.Value, ref next);
                }
                
                if (validation.ExclusiveMaximum.HasValue)
                {
                    GenerateDecimalValidation(OpCodes.Blt_S, validation.ExclusiveMaximum.Value, ref next);
                }

                if (validation.MultipleOf.HasValue)
                {
                    if (next != null)
                    {
                        _generator.MarkLabel(next.Value);
                    }
            
                    next = _generator.DefineLabel();
                    getValue();
                    EmitValue(type, validation.MultipleOf.Value);
                    _generator.EmitCall(OpCodes.Call, s_decimalRemainder, null);
                    _generator.Emit(OpCodes.Ldsfld, s_decimalZero);
                    _generator.EmitCall(OpCodes.Call, s_decimalComparer, null);
                    _generator.Emit(OpCodes.Brfalse_S, next.Value);
                    
                    error();
                }
            }
            else
            {
                var isBig = IsBigNumber(type);
                
                if (validation.Minimum.HasValue)
                {
                    var code = isBig ? OpCodes.Bge_Un_S : OpCodes.Bge_S;
                    GenerateNumberValidation(code, validation.Minimum.Value, ref next);
                }
                
                if (validation.Maximum.HasValue)
                {
                    var code = isBig ? OpCodes.Ble_Un_S : OpCodes.Ble_S;
                    GenerateNumberValidation(code, validation.Maximum.Value, ref next);
                }
                
                if (validation.ExclusiveMinimum.HasValue)
                {
                    var code = isBig ? OpCodes.Bgt_Un_S : OpCodes.Bgt_S;
                    GenerateNumberValidation(code, validation.ExclusiveMinimum.Value, ref next);
                }
                
                if (validation.ExclusiveMaximum.HasValue)
                {
                    var code = isBig ? OpCodes.Blt_Un_S : OpCodes.Blt_S;
                    GenerateNumberValidation(code, validation.ExclusiveMaximum.Value, ref next);
                }

                if (validation.MultipleOf.HasValue)
                {
                    if (next != null)
                    {
                        _generator.MarkLabel(next.Value);
                    }
                
                    next = _generator.DefineLabel();
                    getValue();
                    EmitValue(type, validation.MultipleOf.Value);
                    if (!IsBigNumber(type) || type == typeof(ulong))
                    {
                        var rem = OpCodes.Rem;
                        if (type == typeof(uint) || type == typeof(ulong))
                        {
                            rem = OpCodes.Rem_Un;
                        }
                        
                        _generator.Emit(rem);
                        _generator.Emit(OpCodes.Brfalse_S, next.Value);
                    }
                    else
                    {
                        _generator.Emit(OpCodes.Rem);
                        if (type == typeof(float))
                        {
                            _generator.Emit(OpCodes.Ldc_R4 , (float)0);
                        }
                        else
                        {
                            _generator.Emit(OpCodes.Ldc_R8, (double)0);
                        }
                        
                        _generator.Emit(OpCodes.Beq_S, next.Value);
                    }
                   
                    error();
                }
            }
            
            void GenerateNumberValidation(OpCode code, double value, ref Label? next)
            {
                if (next != null)
                {
                    _generator.MarkLabel(next.Value);
                }
            
                next = _generator.DefineLabel();
                getValue();
                EmitValue(type, value);
                _generator.Emit(code, next.Value);
                error();
            }

            void GenerateDecimalValidation(OpCode code, double value, ref Label? next)
            {
                if (next != null)
                {
                    _generator.MarkLabel(next.Value);
                }
            
                next = _generator.DefineLabel();
                getValue();
                EmitValue(type, value);
                _generator.EmitCall(OpCodes.Call, s_decimalComparer, null);
                _generator.Emit(OpCodes.Ldc_I4_0);
                _generator.Emit(code, next.Value);
                
                error();
            }
            
            void EmitValue(Type fieldType, double value)
            {
                if (fieldType == typeof(byte)
                    || fieldType == typeof(sbyte)
                    || fieldType == typeof(short)
                    || fieldType == typeof(ushort)
                    || fieldType == typeof(int))
                {
                    var convert = Convert.ToInt32(value);
                    if (convert <= 127)
                    {
                        _generator.Emit(OpCodes.Ldc_I4_S, convert);
                    }
                    else
                    {
                        _generator.Emit(OpCodes.Ldc_I4, convert);
                    }
                }
                else if (fieldType == typeof(uint))
                {
                    var convert = Convert.ToUInt32(value);
                    if (convert >= -128 || convert <= 127)
                    {
                        _generator.Emit(OpCodes.Ldc_I4_S, convert);
                    }
                    else if(convert <= int.MaxValue)
                    {
                        _generator.Emit(OpCodes.Ldc_I4, convert);
                    }
                    else if(convert < uint.MaxValue)
                    {
                        var number = (convert - int.MaxValue) + int.MinValue; 
                        _generator.Emit(OpCodes.Ldc_I4, number);
                    }
                    else
                    {
                        _generator.Emit(OpCodes.Ldc_I4_M1);
                    }
                }
                else if (fieldType == typeof(long))
                {
                    var convert = Convert.ToInt64(value);
                    _generator.Emit(OpCodes.Ldc_I8, convert);
                }
                else if (fieldType == typeof(ulong))
                {
                    var convert = Convert.ToUInt64(value);
                    if (convert <= 127)
                    {
                        _generator.Emit(OpCodes.Ldc_I4_S, (int)convert);
                        _generator.Emit(OpCodes.Conv_I8);
                    }
                    else if (convert <= uint.MaxValue)
                    {
                        _generator.Emit(OpCodes.Ldc_I4, (int)convert);
                        _generator.Emit(OpCodes.Conv_I8);
                    }
                    else if (convert <= long.MaxValue)
                    {
                        _generator.Emit(OpCodes.Ldc_I8, convert);
                    }
                    else if (convert == ulong.MaxValue)
                    {
                        _generator.Emit(OpCodes.Ldc_I4_M1);
                    }
                    else if(convert <= ulong.MaxValue - 127)
                    {
                        var number = -(long)(ulong.MaxValue  - convert); 
                        _generator.Emit(OpCodes.Ldc_I4_S, number);
                        _generator.Emit(OpCodes.Conv_I8);
                    }
                    else if(convert <= ulong.MaxValue - int.MaxValue)
                    {
                        var number = -(long)(ulong.MaxValue  - convert); 
                        _generator.Emit(OpCodes.Ldc_I4, number);
                        _generator.Emit(OpCodes.Conv_I8);
                    }
                    else
                    {
                        var number = -(long)(ulong.MaxValue  - convert); 
                        _generator.Emit(OpCodes.Ldc_I8, number);
                    }
                }
                else if (fieldType == typeof(float))
                {
                    var convert = Convert.ToSingle(value);
                    _generator.Emit(OpCodes.Ldc_R4, convert);
                }
                else if(fieldType == typeof(double))
                {
                    var convert = Convert.ToDouble(value);
                    _generator.Emit(OpCodes.Ldc_R8, convert);
                }
                else
                {
                    _generator.Emit(OpCodes.Ldstr, Convert.ToString(value, CultureInfo.InvariantCulture));
                    _generator.EmitCall(OpCodes.Call, s_toDecimal, null);
                }
            }
        }

        private void AddStringValidation(Validation validation, Action getValue, Action error, ref Label? next)
        {
            if (validation.MinimumLength.HasValue)
            {
                GenerateNumberValidation(OpCodes.Bge_S, validation.MinimumLength.Value, ref next);
            }
            
            if (validation.MaximumLength.HasValue)
            {
                GenerateNumberValidation(OpCodes.Ble_S, validation.MaximumLength.Value, ref next);
            }

            if (validation.Pattern != null)
            {
                var regex = _builder.DefineField($"_regex", typeof(Regex),
                    FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);

                if (next != null)
                {
                    _generator.MarkLabel(next.Value);
                }

                next = _generator.DefineLabel();
                var isNull = _generator.DefineLabel();
                getValue();
                _generator.Emit(OpCodes.Brfalse_S, isNull);
                        
                _generator.Emit(OpCodes.Ldsfld, regex);
                getValue();
                _generator.EmitCall(OpCodes.Callvirt, s_match, null);
                _generator.EmitCall(OpCodes.Callvirt, s_success, null);
                _generator.Emit(OpCodes.Brtrue_S, next.Value);
                        
                _generator.MarkLabel(isNull);
                error();

                var staticConstructor = _builder.DefineTypeInitializer();
                var constructorIL = staticConstructor.GetILGenerator();
                
                constructorIL.Emit(OpCodes.Ldstr, validation.Pattern);
                constructorIL.Emit(OpCodes.Ldc_I4_8);
                constructorIL.Emit(OpCodes.Newobj, s_regexConstructor);
                constructorIL.Emit(OpCodes.Stsfld, regex);
                constructorIL.Emit(OpCodes.Ret);
            }
            
            void GenerateNumberValidation(OpCode code, int value, ref Label? next)
            {
                if (next != null)
                {
                    _generator.MarkLabel(next.Value);
                }
            
                next = _generator.DefineLabel();
                var nextCheckNull = _generator.DefineLabel();
                
                getValue();
                _generator.Emit(OpCodes.Brfalse_S, nextCheckNull);
                
                getValue(); 
                
                _generator.EmitCall(OpCodes.Callvirt, s_getLength, null);
                _generator.Emit(OpCodes.Ldc_I4, value);
                _generator.Emit(code, next.Value);
                
                _generator.MarkLabel(nextCheckNull);
                error();
            }
        }
        
        private static bool IsString(Type type)
            => type == typeof(string);
        
        private static bool IsNumber(Type type)
            => type == typeof(int)
               || type == typeof(uint)
               || type == typeof(long)
               || type == typeof(ulong)
               || type == typeof(short)
               || type == typeof(ushort)
               || type == typeof(double)
               || type == typeof(float)
               || type == typeof(decimal)
               || type == typeof(byte)
               || type == typeof(sbyte);
        
        private static bool IsBigNumber(Type parameterType)
            => parameterType == typeof(ulong)
               || parameterType == typeof(float)
               || parameterType == typeof(double);
    }

    public class Validation
    {
        public Validation(double? minimum, double? maximum, 
            double? exclusiveMinimum, double? exclusiveMaximum, double? multipleOf, 
            int? minimumLength, int? maximumLength, string? pattern)
        {
            Minimum = minimum;
            Maximum = maximum;
            ExclusiveMinimum = exclusiveMinimum;
            ExclusiveMaximum = exclusiveMaximum;
            MultipleOf = multipleOf;
            MinimumLength = minimumLength;
            MaximumLength = maximumLength;
            Pattern = pattern;
        }

        public double? Minimum { get; }
        public double? Maximum { get; }
        public double? ExclusiveMinimum { get; }
        public double? ExclusiveMaximum { get; }
        public double? MultipleOf { get; }
        public int? MinimumLength { get; }
        public int? MaximumLength { get; }
        public string? Pattern { get; }
    }
}
