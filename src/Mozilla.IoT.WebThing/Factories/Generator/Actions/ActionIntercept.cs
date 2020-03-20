using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Actions.Parameters.Boolean;
using Mozilla.IoT.WebThing.Actions.Parameters.Number;
using Mozilla.IoT.WebThing.Actions.Parameters.String;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Actions
{
    /// <inheritdoc />
    public class ActionIntercept : IActionIntercept
    {
        private const MethodAttributes s_getSetAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        private static readonly ConstructorInfo s_valueTask = typeof(ValueTask).GetConstructor(new[] {typeof(Task)})!;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ThingOption _option;
        
        /// <summary>
        /// The <see cref="ActionCollection"/> Create, map by action name.
        /// </summary>
        public  Dictionary<string, ActionCollection> Actions { get; }

        /// <summary>
        /// Initialize a new instance of <see cref="ActionIntercept"/>.
        /// </summary>
        /// <param name="moduleBuilder"></param>
        /// <param name="option"></param>
        public ActionIntercept(ModuleBuilder moduleBuilder, ThingOption option)
        {
            _option = option;
            _moduleBuilder = moduleBuilder;
            Actions = option.IgnoreCase ? new Dictionary<string, ActionCollection>(StringComparer.InvariantCultureIgnoreCase) 
                : new Dictionary<string, ActionCollection>();
        }

        /// <inheritdoc/> 
        public void Before(Thing thing)
        {
        }

        /// <inheritdoc/> 
        public void After(Thing thing)
        {
        }

        /// <inheritdoc/> 
        public void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInformation)
        {
            var name = actionInformation?.Name ?? action.Name;
            var thingType = thing.GetType();
            
            var inputBuilder = CreateInput(action);
            var (actionInfoBuilder, inputProperty) = CreateActionInfo(action, inputBuilder, thingType, name);
            var factory = CreateActionInfoFactory(actionInfoBuilder, inputBuilder, inputProperty);
            var parameters = GetParameters(action);
            
            Actions.Add(name, new ActionCollection(new DictionaryInputConvert(parameters), 
                (IActionInfoFactory)Activator.CreateInstance(factory)!));
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

        private (TypeBuilder, PropertyBuilder) CreateActionInfo(MethodInfo action, TypeBuilder inputType, Type thingType, string actionName)
        {
            var actionInfo = _moduleBuilder.DefineType($"{thingType.Name}{action.Name}ActionInfo",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass,
                typeof(ActionInfo));
            
            var input = CreateProperty(actionInfo, "input", inputType);
            
            var getProperty = actionInfo.DefineMethod(nameof(ActionInfo.GetActionName), 
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, 
                typeof(string), Type.EmptyTypes);

            getProperty.GetILGenerator().Return(actionName);
            
            CreateInternalExecuteAsync(action, actionInfo, inputType, input, thingType);
            actionInfo.CreateType();
            return (actionInfo, input);
        }

        private TypeBuilder CreateActionInfoFactory(Type actionInfo, Type inputType, PropertyInfo inputProperty)
        {
            var actionInfoFactory = _moduleBuilder.DefineType($"{actionInfo.Name}Factory",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass,
                null, new []{ typeof(IActionInfoFactory) });

            var createMethod = actionInfoFactory.DefineMethod(nameof(IActionInfoFactory.CreateActionInfo),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.Standard, 
                typeof(ActionInfo), 
                new[] {typeof(Dictionary<string, object>)});

            var generator = createMethod.GetILGenerator();

            generator.NewObj(actionInfo.GetConstructors()[0]);
            generator.NewObj(inputType.GetConstructors()[0], true);

            foreach (var property in inputType.GetProperties())
            {
                generator.SetProperty(property);
            }
            
            generator.Call(inputProperty.SetMethod!);
            generator.Emit(OpCodes.Ret);

            actionInfoFactory.CreateType();
            return actionInfoFactory;
        }

        private static void CreateInternalExecuteAsync(MethodInfo action, TypeBuilder actionInfo, TypeBuilder input, PropertyInfo inputProperty, Type thingType)
        {
            var execute = actionInfo.DefineMethod("InternalExecuteAsync",
                MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    typeof(ValueTask), new [] { typeof(Thing), typeof(IServiceProvider) });

            var generator = execute.GetILGenerator();
            generator.CastFirstArg(thingType);
            
            var inputProperties = input.GetProperties();
            var counter = 0;
            
            foreach (var parameter in action.GetParameters())
            {
                if (parameter.GetCustomAttribute<FromServicesAttribute>() != null)
                {
                    generator.LoadFromService(parameter.ParameterType);
                }
                else if(parameter.ParameterType == typeof(CancellationToken))
                {
                    generator.LoadCancellationToken();
                }
                else
                {
                    var property = inputProperties[counter++];
                    generator.LoadFromInput(inputProperty.GetMethod!, property.GetMethod!);
                }
            }
            
            generator.Call(action);
            if (action.ReturnType == typeof(ValueTask))
            {
               generator.Emit(OpCodes.Ret);
            }
            else if(action.ReturnType == typeof(Task))
            {
                generator.Return(s_valueTask);
            }
            else
            {
                var valueTask = generator.DeclareLocal(typeof(ValueTask));
                generator.Return(valueTask);
            }
        }

        private IReadOnlyDictionary<string, IActionParameter> GetParameters(MethodInfo action)
        {
            var parameters = _option.IgnoreCase ? new Dictionary<string, IActionParameter>(StringComparer.InvariantCultureIgnoreCase)
                : new Dictionary<string, IActionParameter>();
            
            foreach (var parameter in action.GetParameters().Where(IsValidParameter))
            {

                IActionParameter actionParameter;
                var parameterType = parameter.ParameterType.GetUnderlyingType();
                var validation = ToValidation(parameter.GetCustomAttribute<ThingParameterAttribute>());
                var isNullable = parameterType == typeof(string) || parameter.ParameterType.IsNullable() || validation.HasNullValueOnEnum;

                if (parameterType == typeof(bool))
                {
                    actionParameter = new ParameterBoolean(isNullable);
                }
                else if (parameterType == typeof(string))
                {
                    actionParameter = new ParameterString(isNullable,
                        validation.MinimumLength, validation.MaximumLength, validation.Pattern,
                        validation.Enums?.Select(Convert.ToString).ToArray()!);
                }
                else if (parameterType == typeof(Guid))
                {
                    actionParameter = new ParameterGuid(isNullable,
                        validation.Enums?.Select(x => Guid.Parse(x.ToString()!)).ToArray());
                }
                else if (parameterType == typeof(TimeSpan))
                {
                    actionParameter = new ParameterTimeSpan(isNullable,
                        validation.Enums?.Select(x => TimeSpan.Parse(x.ToString()!)).ToArray());
                }
                else if (parameterType == typeof(DateTime))
                {
                    actionParameter = new ParameterDateTime(isNullable,
                        validation.Enums?.Select(Convert.ToDateTime).ToArray());
                }
                else if (parameterType == typeof(DateTimeOffset))
                {
                    actionParameter = new ParameterDateTimeOffset(isNullable,
                        validation.Enums?.Select(x => DateTimeOffset.Parse(x.ToString()!)).ToArray());
                }
                else
                {
                    var minimum = validation.Minimum;
                    var maximum = validation.Maximum;
                    var multipleOf = validation.MultipleOf;
                    var enums = validation.Enums;

                    if (validation.ExclusiveMinimum.HasValue)
                    {
                        minimum = validation.ExclusiveMinimum!.Value + 1;
                    }

                    if (validation.ExclusiveMaximum.HasValue)
                    {
                        maximum = validation.ExclusiveMaximum!.Value - 1;
                    }

                    if (parameterType == typeof(byte))
                    {
                        var min = minimum.HasValue ? new byte?(Convert.ToByte(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new byte?(Convert.ToByte(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf!.Value)) : null;

                        actionParameter = new ParameterByte(isNullable,
                            min, max, multi, enums?.Select(Convert.ToByte).ToArray());
                    }
                    else if (parameterType == typeof(sbyte))
                    {
                        var min = minimum.HasValue ? new sbyte?(Convert.ToSByte(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new sbyte?(Convert.ToSByte(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new sbyte?(Convert.ToSByte(multipleOf!.Value)) : null;

                        actionParameter = new ParameterSByte(isNullable,
                            min, max, multi, enums?.Select(Convert.ToSByte).ToArray());
                    }
                    else if (parameterType == typeof(short))
                    {
                        var min = minimum.HasValue ? new short?(Convert.ToInt16(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new short?(Convert.ToInt16(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new short?(Convert.ToInt16(multipleOf!.Value)) : null;

                        actionParameter = new ParameterShort(isNullable,
                            min, max, multi, enums?.Select(Convert.ToInt16).ToArray());
                    }
                    else if (parameterType == typeof(ushort))
                    {
                        var min = minimum.HasValue ? new ushort?(Convert.ToUInt16(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new ushort?(Convert.ToUInt16(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf!.Value)) : null;

                        actionParameter = new ParameterUShort(isNullable,
                            min, max, multi, enums?.Select(Convert.ToUInt16).ToArray());
                    }
                    else if (parameterType == typeof(int))
                    {
                        var min = minimum.HasValue ? new int?(Convert.ToInt32(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new int?(Convert.ToInt32(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new int?(Convert.ToInt32(multipleOf!.Value)) : null;

                        actionParameter = new ParameterInt(isNullable,
                            min, max, multi, enums?.Select(Convert.ToInt32).ToArray());
                    }
                    else if (parameterType == typeof(uint))
                    {
                        var min = minimum.HasValue ? new uint?(Convert.ToUInt32(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new uint?(Convert.ToUInt32(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new uint?(Convert.ToUInt32(multipleOf!.Value)) : null;

                        actionParameter = new ParameterUInt(isNullable,
                            min, max, multi, enums?.Select(Convert.ToUInt32).ToArray());
                    }
                    else if (parameterType == typeof(long))
                    {
                        var min = minimum.HasValue ? new long?(Convert.ToInt64(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new long?(Convert.ToInt64(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new long?(Convert.ToInt64(multipleOf!.Value)) : null;

                        actionParameter = new ParameterLong(isNullable,
                            min, max, multi, enums?.Select(Convert.ToInt64).ToArray());
                    }
                    else if (parameterType == typeof(ulong))
                    {
                        var min = minimum.HasValue ? new ulong?(Convert.ToUInt64(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new ulong?(Convert.ToUInt64(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf!.Value)) : null;

                        actionParameter = new ParameterULong(isNullable,
                            min, max, multi, enums?.Select(Convert.ToUInt64).ToArray());
                    }
                    else if (parameterType == typeof(float))
                    {
                        var min = minimum.HasValue ? new float?(Convert.ToSingle(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new float?(Convert.ToSingle(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new float?(Convert.ToSingle(multipleOf!.Value)) : null;

                        actionParameter = new ParameterFloat(isNullable,
                            min, max, multi, enums?.Select(Convert.ToSingle).ToArray());
                    }
                    else if (parameterType == typeof(double))
                    {
                        var min = minimum.HasValue ? new double?(Convert.ToDouble(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new double?(Convert.ToDouble(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new double?(Convert.ToDouble(multipleOf!.Value)) : null;

                        actionParameter = new ParameterDouble(isNullable,
                            min, max, multi, enums?.Select(Convert.ToDouble).ToArray());
                    }
                    else
                    {
                        var min = minimum.HasValue ? new decimal?(Convert.ToDecimal(minimum!.Value)) : null;
                        var max = maximum.HasValue ? new decimal?(Convert.ToDecimal(maximum!.Value)) : null;
                        var multi = multipleOf.HasValue ? new decimal?(Convert.ToDecimal(multipleOf!.Value)) : null;

                        actionParameter = new ParameterDecimal(isNullable,
                            min, max, multi, enums?.Select(Convert.ToDecimal).ToArray());
                    }
                }

                parameters.Add(parameter.Name!, actionParameter);
            }

            return parameters;

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
