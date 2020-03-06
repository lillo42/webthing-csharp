using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    public class IlFactory
    {
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
            _generator.Emit(OpCodes.Ldc_I4_S, value);
            _generator.Emit(OpCodes.Bne_Un_S, _next);

            _sb.Append("lcd.i4.s ").AppendLine(value.ToString());
            _sb.AppendLine("bne.un.s NEXT");
            
            
            _sb.AppendLine();
        }
        
        public void IfIsDifferent(LocalBuilder local, MethodInfo getter, int value)
        {
            SetNext();
            GetLocal(local);
            Call(getter);
            _generator.Emit(OpCodes.Ldc_I4_S, value);
            _generator.Emit(OpCodes.Beq_S, _next);
            
            _sb.Append("lcd.i4.s ").AppendLine(value.ToString());
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
    }
}
