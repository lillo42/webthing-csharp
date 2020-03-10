using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ThingOption _option;
        public  Dictionary<string, ActionCollection> Actions { get; }

        public ActionIntercept(ModuleBuilder moduleBuilder, ThingOption option)
        {
            _option = option;
            _moduleBuilder = moduleBuilder;
            Actions = option.IgnoreCase ? new Dictionary<string, ActionCollection>(StringComparer.InvariantCultureIgnoreCase) 
                : new Dictionary<string, ActionCollection>();
        }

        public void Before(Thing thing)
        {
        }

        public void After(Thing thing)
        {
        }

        public void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInfo)
        {
            var name = actionInfo?.Name ?? action.Name;
            var thingType = thing.GetType();
            
            var inputBuilder = CreateInput(action);
            var actionInfoBuilder = CreateActionInfo(action, inputBuilder, thingType, name);
        }

        private TypeBuilder CreateInput(MethodInfo action)
        {
            var inputBuilder = _moduleBuilder.DefineType($"{action.Name}Input",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass);
            
            var parameters = action.GetParameters();
            foreach (var parameter in parameters.Where(IsValidParameter))
            { 
                CreateProperty(inputBuilder, parameter.Name!, parameter.ParameterType);
            }

            inputBuilder.CreateType();

            return inputBuilder;
        }
        private TypeBuilder CreateActionInfo(MethodInfo action, TypeBuilder inputType, Type thingType, string actionName)
        {
            var actionInfo = _moduleBuilder.DefineType($"{thingType.Name}{action.Name}ActionInfo",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass,
                typeof(ActionInfo));
            
            CreateProperty(actionInfo, "input", inputType);
            
            var getProperty = actionInfo.DefineMethod(nameof(ActionInfo.GetActionName), 
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, 
                typeof(string), Type.EmptyTypes);

            getProperty.GetILGenerator().Return(actionName);
            actionInfo.CreateType();
            return actionInfo;
        }
        private TypeBuilder CreateActionCollection(MethodInfo action, TypeBuilder actionInfo, TypeBuilder input, Type thing)
        {
            var collection = _moduleBuilder.DefineType($"{thing.Name}{action.Name}ActionCollection",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass,
                typeof(ActionCollection));
            
            CreateAdd(action, collection, actionInfo, input);

            collection.CreateType();
            return collection;
        }
        
        private void CreateAdd(MethodInfo action, TypeBuilder collection, TypeBuilder actionInfo, TypeBuilder input)
        {
            var method = collection.DefineMethod("IsValid",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |MethodAttributes.Virtual,
                CallingConventions.Standard, typeof(ActionInfo), 
                JsonElementMethods.ArrayOfJsonElement);

            var patterns = new LinkedList<(string pattern, FieldBuilder regex)>();
            var factory = new IlFactory(method.GetILGenerator());

            var info = factory.CreateLocalField(actionInfo);
            factory.SetArgToLocal(info);
            
            foreach (var parameter in action.GetParameters().Where(IsValidParameter))
            {
                var validator = ToValidation(parameter.GetCustomAttribute<ThingParameterAttribute>());
                FieldBuilder? regex = null;
                if (validator.Pattern != null)
                {
                    regex = collection.DefineField($"_regex{parameter.Name}", typeof(Regex), FieldAttributes.Private | FieldAttributes.InitOnly | FieldAttributes.Static);
                    patterns.AddLast((validator.Pattern, regex));
                }

                var parameterType = parameter.ParameterType.GetUnderlyingType();
                var validation = ToValidation(parameter.GetCustomAttribute<ThingParameterAttribute>());
                
                
                
                
            }

            static Validation ToValidation(ThingParameterAttribute? validation)
            {
                return new Validation(validation?.MinimumValue, validation?.MaximumValue,
                    validation?.ExclusiveMinimumValue, validation?.ExclusiveMaximumValue,
                    validation?.MultipleOfValue,
                    validation?.MinimumLengthValue, validation?.MaximumLengthValue,
                    validation?.Pattern, validation?.Enum);
            }
        }

        private static bool IsValidParameter(ParameterInfo parameter)
            => parameter.GetCustomAttribute<FromServicesAttribute>() == null &&
               parameter.ParameterType != typeof(CancellationToken);
        private static PropertyBuilder CreateProperty(TypeBuilder builder, string fieldName, Type type)
        {
            var field = builder.DefineField($"_{fieldName}", type, FieldAttributes.Private);
            var parameterName = fieldName.FirstCharToUpper();
            var propertyBuilder = builder.DefineProperty(parameterName, 
                PropertyAttributes.HasDefault,
                type, null);

            var getProperty = builder.DefineMethod($"get_{parameterName}", s_getSetAttributes,
                type, Type.EmptyTypes);

            getProperty.GetILGenerator().Return(field);

            // Define the "set" accessor method for CustomerName.
            var setProperty = builder.DefineMethod($"set_{parameterName}", s_getSetAttributes,
                null, new[] {type});

            setProperty.GetILGenerator().Set(field);

            propertyBuilder.SetGetMethod(getProperty);
            propertyBuilder.SetSetMethod(setProperty);
            
            return propertyBuilder;
        }
    }
}
