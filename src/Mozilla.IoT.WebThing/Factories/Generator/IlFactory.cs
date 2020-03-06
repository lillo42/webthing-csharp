using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    public class IlFactory
    {
        private static readonly MethodInfo s_toDecimal = typeof(Convert).GetMethod(nameof(Convert.ToDecimal), new[] { typeof(string) });
        private static readonly MethodInfo s_decimalComparer = typeof(decimal).GetMethod(nameof(decimal.Compare), new[] { typeof(decimal), typeof(decimal) });
        private static readonly MethodInfo s_decimalRemainder = typeof(decimal).GetMethod(nameof(decimal.Remainder), new[] { typeof(decimal), typeof(decimal) });
        private static readonly FieldInfo s_decimalZero = typeof(decimal).GetField(nameof(decimal.Zero));

        private static readonly MethodInfo s_stringComparer = typeof(string).GetMethod(nameof(string.Compare), new[] { typeof(string), typeof(string) });

        private readonly ILGenerator _generator;
        public readonly StringBuilder _sb = new StringBuilder();
        private Label _next;

        public IlFactory(ILGenerator generator)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public void Return(int result)
        {
            _generator.Emit(OpCodes.Ldc_I4_S, result);
            _generator.Emit(OpCodes.Ret);

            _sb.Append("ldc.i4.s ").AppendLine(result.ToString());
            _sb.AppendLine("ret");
            _sb.AppendLine();
        }

        public LocalBuilder CreateLocalField(Type local)
            => _generator.DeclareLocal(local);

        public void SetArgToLocal(LocalBuilder local)
        {
            _generator.Emit(OpCodes.Ldarg_1);
            _generator.Emit(OpCodes.Stloc_S, local.LocalIndex);

            _sb.AppendLine("ldarg.1");
            _sb.Append("stloc.s ").AppendLine(local.LocalIndex.ToString());
        }

        public void GetLocal(LocalBuilder local)
        {
            _generator.Emit(OpCodes.Ldloca_S, local.LocalIndex);
            _sb.Append("ldloca.s ").AppendLine(local.LocalIndex.ToString());
        }

        private void SetNext()
        {
            _next = _generator.DefineLabel();
        }

        private void Call(MethodInfo method)
        {
            var call = method.DeclaringType.IsClass ? OpCodes.Callvirt : OpCodes.Call;
            _generator.EmitCall(call, method, null);

            if (call == OpCodes.Call)
            {
                _sb.Append("call ");
            }
            else
            {
                _sb.Append("callvirt ");
            }

            _sb.AppendLine(method.Name);
        }

        #region If

        public void IfIsEquals(LocalBuilder local, MethodInfo getter, int value)
        {
            SetNext();
            GetLocal(local);
            Call(getter);
            _generator.Emit(OpCodes.Bne_Un_S, _next);

            _sb.Append("lcd.i4.s ").AppendLine(value.ToString());
            _sb.AppendLine("bne.un.s NEXT");

            _sb.AppendLine();
        }

        public void IfIsLessThan(LocalBuilder local, MethodInfo getter, int value)
        {
            SetNext();
            GetLocal(local);
            Call(getter);
            EmitNumber(value, typeof(int));
            _generator.Emit(OpCodes.Bge_S, _next);
            _sb.AppendLine("bge.S NEXT");

            _sb.AppendLine();
        }

        public void IfIsLessThan(LocalBuilder local, double value)
        {
            SetNext();
            GetLocal(local);
            EmitNumber(value, local.LocalType);

            if (local.LocalType == typeof(decimal))
            {
                Call(s_decimalComparer);
                _generator.Emit(OpCodes.Ldc_I4_0);
                _sb.AppendLine("ldc.i4.0");
                _generator.Emit(OpCodes.Bge_S, _next);
                _sb.AppendLine("bge.S NEXT");
            }
            else if (IsBigNumber(local.LocalType))
            {
                _generator.Emit(OpCodes.Bge_Un_S, _next);
                _sb.AppendLine("bge.un.S NEXT");
            }
            else
            {
                _generator.Emit(OpCodes.Bge_S, _next);
                _sb.AppendLine("bge.S NEXT");
            }

            _sb.AppendLine();
        }

        public void IfIsLessOrEqualThan(LocalBuilder local, double value)
        {
            SetNext();
            GetLocal(local);
            EmitNumber(value, local.LocalType);
            if (local.LocalType == typeof(decimal))
            {
                Call(s_decimalComparer);
                _generator.Emit(OpCodes.Ldc_I4_0);
                _sb.AppendLine("ldc.i4.0");
                _generator.Emit(OpCodes.Bgt_S, _next);
                _sb.AppendLine("bge.S NEXT");
            }
            else if (IsBigNumber(local.LocalType))
            {
                _generator.Emit(OpCodes.Bgt_Un_S, _next);
                _sb.AppendLine("bgt.un.S NEXT");
            }
            else
            {
                _generator.Emit(OpCodes.Bgt_S, _next);
                _sb.AppendLine("bgt.S NEXT");
            }

            _sb.AppendLine();
        }

        public void IfIsGreaterThan(LocalBuilder local, double value)
        {
            SetNext();
            GetLocal(local);
            EmitNumber(value, local.LocalType);
            if (local.LocalType == typeof(decimal))
            {
                Call(s_decimalComparer);
                _generator.Emit(OpCodes.Ldc_I4_0);
                _sb.AppendLine("ldc.i4.0");
                _generator.Emit(OpCodes.Bgt_S, _next);
                _sb.AppendLine("ble.S NEXT");
            }
            else if (IsBigNumber(local.LocalType))
            {
                _generator.Emit(OpCodes.Ble_Un_S, _next);
                _sb.AppendLine("ble.un.S NEXT");
            }
            else
            {
                _generator.Emit(OpCodes.Ble_S, _next);
                _sb.AppendLine("ble.S NEXT");
            }

            _sb.AppendLine();
        }

        public void IfIsGreaterThan(LocalBuilder local, MethodInfo getter, int value)
        {
            SetNext();
            GetLocal(local);
            Call(getter);
            EmitNumber(value, typeof(int));
            _generator.Emit(OpCodes.Ble_S, _next);
            _sb.AppendLine("ble.S NEXT");

            _sb.AppendLine();
        }

        public void IfIsGreaterOrEqualThan(LocalBuilder local, double value)
        {
            SetNext();
            GetLocal(local);
            EmitNumber(value, local.LocalType);
            if (local.LocalType == typeof(decimal))
            {
                Call(s_decimalComparer);
                _generator.Emit(OpCodes.Ldc_I4_0);
                _sb.AppendLine("ldc.i4.0");
                _generator.Emit(OpCodes.Bgt_S, _next);
                _sb.AppendLine("ble.S NEXT");
            }
            else if (IsBigNumber(local.LocalType))
            {
                _generator.Emit(OpCodes.Blt_Un_S, _next);
                _sb.AppendLine("ble.un.S NEXT");
            }
            else
            {
                _generator.Emit(OpCodes.Blt_S, _next);
                _sb.AppendLine("ble.S NEXT");
            }

            _sb.AppendLine();
        }

        public void IfIsNotMultipleOf(LocalBuilder local, double value)
        {
            SetNext();
            GetLocal(local);
            EmitNumber(value, local.LocalType);
            if (local.LocalType == typeof(decimal))
            {
                Call(s_decimalRemainder);
                _generator.Emit(OpCodes.Ldsfld, s_decimalZero);
                _sb.AppendLine("ldsfld DECIMAL ZERO");
                Call(s_decimalComparer);
                _generator.Emit(OpCodes.Brfalse_S, _next);
                _sb.AppendLine("brfalse.s NEXT");
            }
            else if (!IsBigNumber(local.LocalType) || local.LocalType == typeof(ulong))
            {
                if (local.LocalType == typeof(uint) || local.LocalType == typeof(ulong))
                {
                    _generator.Emit(OpCodes.Rem_Un);
                    _sb.AppendLine("rem.un");
                }
                else
                {
                    _generator.Emit(OpCodes.Rem);
                    _sb.AppendLine("rem");
                }

                _generator.Emit(OpCodes.Brfalse_S, _next);
                _sb.AppendLine("brfalse.s NEXT");
            }
            else
            {
                _generator.Emit(OpCodes.Rem);
                _sb.AppendLine("rem");
                if (local.LocalType == typeof(float))
                {
                    _generator.Emit(OpCodes.Ldc_R4, (float)0);
                    _sb.AppendLine("ldc.r4 0");
                }
                else
                {
                    _generator.Emit(OpCodes.Ldc_R8, (double)0);
                    _sb.AppendLine("ldc.r8 0");
                }

                _generator.Emit(OpCodes.Beq_S, _next);
                _sb.AppendLine("beq.s NEXT");
            }

            _sb.AppendLine();
        }

        public void IfIsDifferent(LocalBuilder local, MethodInfo getter, int value)
        {
            SetNext();
            GetLocal(local);
            Call(getter);
            _generator.Emit(OpCodes.Ldc_I4_S, value);
            _sb.Append("lcd.i4.s ").AppendLine(value.ToString());
            _generator.Emit(OpCodes.Beq_S, _next);
            _sb.AppendLine("beq.s NEXT");

            _sb.AppendLine();
        }

        public void IfIsDifferent(LocalBuilder local, MethodInfo getter, params int[] values)
        {
            SetNext();
            foreach (var value in values)
            {
                GetLocal(local);
                Call(getter);
                _generator.Emit(OpCodes.Ldc_I4_S, value);
                _generator.Emit(OpCodes.Beq_S, _next);

                _sb.Append("lcd.i4.s ").AppendLine(value.ToString());
                _sb.AppendLine("beq.s NEXT");

                _sb.AppendLine();
            }
        }

        public void IfIsDifferent(LocalBuilder local, params object[] values)
        {
            SetNext();
            var hash = new HashSet<object>();
            foreach (var value in values)
            {
                if (value ==  null || !hash.Add(value))
                {
                    continue;
                }
                
                GetLocal(local);
                
                if (local.LocalType == typeof(string))
                {
                    var convert = Convert.ToString(value);
                    _generator.Emit(OpCodes.Ldstr, convert);
                    _generator.EmitCall(OpCodes.Call, s_stringComparer, null);
                    _generator.Emit(OpCodes.Brfalse_S, _next);
                }
                else
                {
                    EmitNumber(value, local.LocalType);
                    _generator.Emit(OpCodes.Beq_S, _next);
                    _sb.AppendLine("beq.s NEXT");
                }

                _sb.AppendLine();
            }
        }

        public void IfTryGetIsFalse(LocalBuilder source, LocalBuilder destiny, MethodInfo getter)
        {
            SetNext();
            _generator.Emit(OpCodes.Ldloca_S, source.LocalIndex);
            _generator.Emit(OpCodes.Ldloca_S, destiny.LocalIndex);

            _sb.Append("ldloca.s ").AppendLine(source.ToString());
            _sb.AppendLine("ldloca.s ").AppendLine(source.ToString());

            Call(getter);
            _generator.Emit(OpCodes.Brtrue_S, _next);
            _sb.Append("brtrue.s NEXT");

            _sb.AppendLine();
        }

        public void EndIf()
        {
            _generator.MarkLabel(_next);
            _sb.AppendLine("MARKING NEXT");
            _sb.AppendLine();
        }

        #endregion

        #region Set

        public void SetNullValue(FieldBuilder field, MethodInfo setter)
        {
            _generator.Emit(OpCodes.Ldarg_0);
            _sb.AppendLine("ldarg.0");
            _generator.Emit(OpCodes.Ldfld, field);
            _sb.Append("ldfld ").AppendLine(field.Name);
            _generator.Emit(OpCodes.Ldnull);
            _sb.AppendLine("ldnull ");
            Call(setter);
            _sb.AppendLine();
        }

        public void SetNullValue(FieldBuilder field, MethodInfo setter, LocalBuilder nullable)
        {
            _generator.Emit(OpCodes.Ldarg_0);
            _sb.AppendLine("ldarg.0");
            _generator.Emit(OpCodes.Ldfld, field);
            _sb.Append("ldfld ").AppendLine(field.Name);
            _generator.Emit(OpCodes.Ldloca_S, nullable.LocalIndex);
            _sb.Append("ldloca.s ").AppendLine(nullable.LocalIndex.ToString());
            _generator.Emit(OpCodes.Initobj, nullable.LocalType);
            _sb.Append("initobj ").AppendLine(nullable.LocalType.ToString());
            _generator.Emit(OpCodes.Ldloc_S, nullable.LocalIndex);
            _sb.Append("ldloc.s ").AppendLine(nullable.LocalIndex.ToString());
            Call(setter);
            _sb.AppendLine();
        }

        public void SetLocal(LocalBuilder origin, MethodInfo getter, LocalBuilder destiny)
        {
            _generator.Emit(OpCodes.Ldloca_S, origin.LocalIndex);
            _sb.Append("ldloca.s ").AppendLine(origin.LocalIndex.ToString());
            Call(getter);
            _generator.Emit(OpCodes.Stloc_S, destiny.LocalIndex);
            _sb.Append("stloc.s ").AppendLine(destiny.LocalIndex.ToString());
        }

        public void SetValue(LocalBuilder origin, FieldBuilder destiny, MethodInfo setter)
        {
            _generator.Emit(OpCodes.Ldarg_0);
            _sb.AppendLine("ldarg.0");
            _generator.Emit(OpCodes.Ldfld, destiny);
            _sb.Append("ldfld ").AppendLine(destiny.Name);
            _generator.Emit(OpCodes.Ldloc_S, origin.LocalIndex);
            _sb.Append("ldloc.s ").AppendLine(origin.LocalIndex.ToString());

            var parameters = setter.GetParameters();
            if (parameters.Length > 0 && parameters[0].ParameterType.IsNullable())
            {
                var constructor = parameters[0].ParameterType.GetConstructors().Last();
                _generator.Emit(OpCodes.Newobj, constructor);
                _sb.Append("newobj ").AppendLine(constructor.Name);
            }

            Call(setter);
        }

        #endregion

        #region Number

        private static bool IsBigNumber(Type parameterType)
            => parameterType == typeof(ulong)
               || parameterType == typeof(float)
               || parameterType == typeof(double);

        private void EmitNumber(object value, Type fieldType)
        {
            if (fieldType == typeof(byte)
                    || fieldType == typeof(sbyte)
                    || fieldType == typeof(short)
                    || fieldType == typeof(ushort)
                    || fieldType == typeof(int))
            {
                var convert = Convert.ToInt32(value);
                if (convert >= -128 && convert <= 127)
                {
                    _generator.Emit(OpCodes.Ldc_I4_S, convert);
                    _sb.Append("ldc.i4.s ").AppendLine(convert.ToString());
                }
                else
                {
                    _generator.Emit(OpCodes.Ldc_I4, convert);
                    _sb.Append("ldc.i4 ").AppendLine(convert.ToString());
                }
            }
            else if (fieldType == typeof(uint))
            {
                var convert = Convert.ToUInt32(value);
                if (convert >= -128 || convert <= 127)
                {
                    _generator.Emit(OpCodes.Ldc_I4_S, convert);
                    _sb.Append("ldc.i4.s ").AppendLine(convert.ToString());
                }
                else if (convert <= int.MaxValue)
                {
                    _generator.Emit(OpCodes.Ldc_I4, convert);
                    _sb.Append("ldc.i4 ").AppendLine(convert.ToString());
                }
                else if (convert < uint.MaxValue)
                {
                    var number = (convert - int.MaxValue) + int.MinValue;
                    _generator.Emit(OpCodes.Ldc_I4, number);
                    _sb.Append("ldc.i4 ").AppendLine(convert.ToString());
                }
                else
                {
                    _generator.Emit(OpCodes.Ldc_I4_M1);
                    _sb.Append("ldc.i4.m1 ").AppendLine(convert.ToString());
                }
            }
            else if (fieldType == typeof(long))
            {
                var convert = Convert.ToInt64(value);
                _generator.Emit(OpCodes.Ldc_I8, convert);
                _sb.Append("ldc.i8 ").AppendLine(convert.ToString());
            }
            else if (fieldType == typeof(ulong))
            {
                var convert = Convert.ToUInt64(value);
                if (convert <= 127)
                {
                    _generator.Emit(OpCodes.Ldc_I4_S, (int)convert);
                    _sb.Append("ldc.i4.s ").AppendLine(convert.ToString());
                    _generator.Emit(OpCodes.Conv_I8);
                    _sb.AppendLine("ldc.i8 ");

                }
                else if (convert <= uint.MaxValue)
                {
                    _generator.Emit(OpCodes.Ldc_I4, (int)convert);
                    _sb.Append("ldc.i4 ").AppendLine(convert.ToString());
                    _generator.Emit(OpCodes.Conv_I8);
                    _sb.AppendLine("ldc.i8 ");
                }
                else if (convert <= long.MaxValue)
                {
                    _generator.Emit(OpCodes.Ldc_I8, convert);
                    _sb.Append("ldc.i8 ").AppendLine(convert.ToString());
                }
                else if (convert == ulong.MaxValue)
                {
                    _generator.Emit(OpCodes.Ldc_I4_M1);
                    _sb.AppendLine("ldc.i4.m1 ");
                }
                else if (convert <= ulong.MaxValue - 127)
                {
                    var number = -(long)(ulong.MaxValue - convert);
                    _generator.Emit(OpCodes.Ldc_I4_S, number);
                    _sb.Append("ldc.i4.s ").AppendLine(convert.ToString());
                    _generator.Emit(OpCodes.Conv_I8);
                    _sb.AppendLine("ldc.i8 ");
                }
                else if (convert <= ulong.MaxValue - int.MaxValue)
                {
                    var number = -(long)(ulong.MaxValue - convert);
                    _generator.Emit(OpCodes.Ldc_I4, number);
                    _sb.Append("ldc.i4 ").AppendLine(convert.ToString());
                    _generator.Emit(OpCodes.Conv_I8);
                    _sb.AppendLine("ldc.i8 ");
                }
                else
                {
                    var number = -(long)(ulong.MaxValue - convert);
                    _generator.Emit(OpCodes.Ldc_I8, number);
                    _sb.Append("ldc.i8 ").AppendLine(convert.ToString());
                }
            }
            else if (fieldType == typeof(float))
            {
                var convert = Convert.ToSingle(value);
                _generator.Emit(OpCodes.Ldc_R4, convert);
                _sb.Append("ldc.r4 ").AppendLine(convert.ToString(CultureInfo.InvariantCulture));
            }
            else if (fieldType == typeof(double))
            {
                var convert = Convert.ToDouble(value);
                _generator.Emit(OpCodes.Ldc_R8, convert);
                _sb.Append("ldc.r8 ").AppendLine(convert.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                _generator.Emit(OpCodes.Ldstr, Convert.ToString(value, CultureInfo.InvariantCulture));
                _generator.EmitCall(OpCodes.Call, s_toDecimal, null);
            }
        }

        #endregion
    }
}
