using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Actions
{
    public class ActionIntercept : IActionIntercept
    {
        private const MethodAttributes s_getSetAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        private readonly ModuleBuilder _moduleBuilder;
        public  Dictionary<string, ActionContext> Actions { get; } = new Dictionary<string, ActionContext>();

        public ActionIntercept(ModuleBuilder moduleBuilder)
        {
            _moduleBuilder = moduleBuilder;
        }

        public void Before(Thing thing)
        {
        }

        public void After(Thing thing)
        {
        }

        public void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInfo)
        {
            var thingType = thing.GetType();
            var inputBuilder = _moduleBuilder.DefineType($"{action.Name}Input",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass);

            var actionBuilder = _moduleBuilder.DefineType($"{thingType.Name}{action.Name}ActionInfo",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass,
                typeof(ActionInfo));

            var name = actionInfo?.Name ?? action.Name;
            CreateActionName(actionBuilder, name);


            var parameters = action.GetParameters();

            var isValid = actionBuilder.DefineMethod("IsValid",
                MethodAttributes.Private | MethodAttributes.Static,
                CallingConventions.Any | CallingConventions.Standard, typeof(bool),
                parameters.Select(x => x.ParameterType).ToArray());
            var isValidIl = isValid.GetILGenerator();

            Label? nextValidation = null;
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                CreateProperty(inputBuilder, parameter.Name, parameter.ParameterType);
                AddParameterValidation(isValidIl, parameter,
                    parameter.GetCustomAttribute<ThingParameterAttribute>(),
                    index + 1, ref nextValidation);
            }

            if (nextValidation.HasValue)
            {
                isValidIl.MarkLabel(nextValidation.Value);
                isValidIl.Emit(OpCodes.Ldc_I4_1);
                isValidIl.Emit(OpCodes.Ret);
            }

            var inputType = inputBuilder.CreateType()!;
            var input = CreateProperty(actionBuilder, "input", inputType);
            CreateInputValidation(actionBuilder, inputBuilder, isValid, input);

            Actions.Add(name, new ActionContext(actionBuilder.CreateType()));
        }

        private static PropertyBuilder CreateProperty(TypeBuilder builder, string fieldName, Type type)
        {
            var fieldBuilder = builder.DefineField($"_{fieldName}", type, FieldAttributes.Private);
            var parameterName = fieldName.FirstCharToUpper();
            var propertyBuilder = builder.DefineProperty(parameterName, PropertyAttributes.HasDefault,
                type, null);

            var getProperty = builder.DefineMethod($"get_{parameterName}", s_getSetAttributes,
                type, Type.EmptyTypes);

            var getPropertyIL = getProperty.GetILGenerator();
            getPropertyIL.Emit(OpCodes.Ldarg_0);
            getPropertyIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getPropertyIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            var setProperty = builder.DefineMethod($"set_{parameterName}", s_getSetAttributes,
                null, new[] {type});

            var setPropertyIL = setProperty.GetILGenerator();

            setPropertyIL.Emit(OpCodes.Ldarg_0);
            setPropertyIL.Emit(OpCodes.Ldarg_1);
            setPropertyIL.Emit(OpCodes.Stfld, fieldBuilder);
            setPropertyIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getProperty);
            propertyBuilder.SetSetMethod(setProperty);
            
            return propertyBuilder;
        }
        private static void CreateActionName(TypeBuilder builder, string value)
        {
            var propertyBuilder = builder.DefineProperty("ActionName", 
                PropertyAttributes.HasDefault | PropertyAttributes.SpecialName,
                typeof(string), null);

            var getProperty = builder.DefineMethod("get_ActionName",  
                MethodAttributes.Family | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                typeof(string), Type.EmptyTypes);

            var getPropertyIL = getProperty.GetILGenerator();
            getPropertyIL.Emit(OpCodes.Ldstr, value);
            getPropertyIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getProperty);
        }

        private static void CreateInputValidation(TypeBuilder builder, TypeBuilder input, MethodInfo isValid,
            PropertyBuilder inputProperty)
        {
            var isInputValidBuilder = builder.DefineMethod(nameof(ActionInfo.IsValid),
                MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), Type.EmptyTypes);
            
            var isInputValid = isInputValidBuilder.GetILGenerator();

            foreach (var property in input.GetProperties())
            {
                isInputValid.Emit(OpCodes.Ldarg_0);
                isInputValid.EmitCall(OpCodes.Call, inputProperty.GetMethod, null);
                isInputValid.EmitCall(OpCodes.Callvirt, property.GetMethod!, null );
            }
            
            isInputValid.EmitCall(OpCodes.Call, isValid, null);
            isInputValid.Emit(OpCodes.Ret);
        }

        private static void AddParameterValidation(ILGenerator il, ParameterInfo parameter, 
            ThingParameterAttribute? validationParameter, int index, ref Label? next)
        {
            if (validationParameter == null)
            {
                return;
            }

            if (IsNumber(parameter.ParameterType))
            {
                if (validationParameter.MinimumValue.HasValue)
                {
                    if (next != null)
                    {
                        il.MarkLabel(next.Value);
                    }
                    next = il.DefineLabel();
                    
                    il.Emit(OpCodes.Ldarg, index);
                    il.Emit(OpCodes.Ldc_I4_S, validationParameter.MinimumValue.Value);
                    il.Emit(OpCodes.Bge_S, next.Value);
                    
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ret);
                }
                
                if (validationParameter.MinimumValue.HasValue)
                {
                    if (next != null)
                    {
                        il.MarkLabel(next.Value);
                    }
                    
                    next = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg, index);
                    il.Emit(OpCodes.Ldc_I4_S, validationParameter.MinimumValue.Value);
                    il.Emit(OpCodes.Ble_S, next.Value);
                    
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ret);
                }
                
                if (validationParameter.MultipleOfValue.HasValue)
                {
                    if (next != null)
                    {
                        il.MarkLabel(next.Value);
                    }
                    next = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg, index);
                    il.Emit(OpCodes.Ldc_I4_S, validationParameter.MultipleOfValue.Value);
                    il.Emit(OpCodes.Rem);
                    
                    il.Emit(OpCodes.Brtrue, next.Value);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ret);
                }
            }
        }
        
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

    }
}
