using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Convertibles;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.Convertibles.Input;
using Mozilla.IoT.WebThing.Json.SchemaValidations;
using Mozilla.IoT.WebThing.Json.SchemaValidations.Input;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <inheritdoc /> 
    public class ActionBuilder : IActionBuilder
    {
        private const MethodAttributes s_getSetAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
        private static readonly ConstructorInfo s_valueTask = typeof(ValueTask).GetConstructor(new[] {typeof(Task)})!;

        private readonly IJsonSchemaValidationFactory _jsonSchemaValidationFactory;
        private readonly IJsonConvertibleFactory _jsonConvertibleFactory;
        private readonly IConvertibleFactory _convertibleFactory;

        private Dictionary<string, IJsonConvertible>? _jsonConvertibles;
        private Dictionary<string, IJsonSchemaValidation>? _jsonSchemaValidations;
        private Dictionary<string, IConvertible?>? _convertibles;

        private Thing? _thing;
        private ThingOption? _option;
        private Type? _thingType;
        private ModuleBuilder? _module;
        private MethodInfo? _action;
        private Dictionary<string, ActionCollection>? _actions;
        private TypeBuilder? _input;
        private string? _name;

        /// <summary>
        /// Initialize a new instance of <see cref="ActionBuilder"/>.
        /// </summary>
        /// <param name="jsonSchemaValidationFactory">The <see cref="IJsonSchemaValidationFactory"/>.</param>
        /// <param name="jsonConvertibleFactory">The <see cref="IJsonConvertibleFactory"/>.</param>
        /// <param name="convertibleFactory">The <see cref="IConvertibleFactory"/>.</param>
        public ActionBuilder(IJsonSchemaValidationFactory jsonSchemaValidationFactory, 
            IJsonConvertibleFactory jsonConvertibleFactory, 
            IConvertibleFactory convertibleFactory)
        {
            _jsonSchemaValidationFactory = jsonSchemaValidationFactory ?? throw new ArgumentNullException(nameof(jsonSchemaValidationFactory));
            _jsonConvertibleFactory = jsonConvertibleFactory ?? throw new ArgumentNullException(nameof(jsonConvertibleFactory));
            _convertibleFactory = convertibleFactory;
        }

        /// <inheritdoc /> 
        public IActionBuilder SetThing(Thing thing)
        {
            _thing = thing;
            return this;
        }

        /// <inheritdoc /> 
        public IActionBuilder SetThingType(Type thingType)
        {
            _thingType = thingType;
            var baseName = $"{thingType.Name}Actions";
            var assemblyName = new AssemblyName($"{baseName}Assembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _module = assemblyBuilder.DefineDynamicModule($"{baseName}Module");

            return this;
        }

        /// <inheritdoc /> 
        public IActionBuilder SetThingOption(ThingOption option)
        {
            _option = option;
            _actions = new Dictionary<string, ActionCollection>(option.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);
            _jsonConvertibles = new Dictionary<string, IJsonConvertible>(option.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);
            _jsonSchemaValidations = new Dictionary<string, IJsonSchemaValidation>(option.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);
            _convertibles = new Dictionary<string, IConvertible?>(option.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);
            return this;
        }

        /// <inheritdoc /> 
        public IActionBuilder Add(MethodInfo action, ThingActionAttribute? attribute)
        {
            if (_thingType == null || _module == null)
            {
                throw new InvalidOperationException($"ThingType is null, call {nameof(SetThingType)} before add");
            }

            if (_actions == null || _option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)} before add");
            }
            
            if (_input != null)
            {
                _input.CreateType();
                var (actionInfoBuilder, inputProperty) = CreateActionInfo(_action!, _input, _thingType, _name!);
                var factory = CreateActionInfoFactory(actionInfoBuilder, _input, inputProperty);
                
                _actions.Add(_name!, new ActionCollection(
                    new SystemTextJsonInputConvertible(_jsonConvertibles!),
                    new InputJsonSchemaValidation(_jsonSchemaValidations!),
                    new InputConvertible(_convertibles!), 
                    (IActionInformationFactory)Activator.CreateInstance(factory)!));
            }
            
            
            _jsonConvertibles = new Dictionary<string, IJsonConvertible>(_option.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);
            _jsonSchemaValidations = new Dictionary<string, IJsonSchemaValidation>(_option.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);
            _convertibles = new Dictionary<string, IConvertible?>(_option.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);
            _name = attribute?.Name ?? action.Name;
            _action = action;
            _input = _module.DefineType($"{action.Name}Input", TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass);

            return this;
        }
        
        /// <inheritdoc /> 
        public IActionBuilder Add(ParameterInfo parameter, JsonSchema jsonSchema)
        {
            if (_input == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(Add)} before add");
            }
            
            CreateProperty(_input, jsonSchema.Name, parameter.ParameterType);
            
            var parameterType = parameter.ParameterType.GetUnderlyingType();
            var code = parameterType.ToTypeCode();
            
            _jsonConvertibles!.Add(jsonSchema.Name, 
                _jsonConvertibleFactory.Create(code, parameterType));
            
            _jsonSchemaValidations!.Add(jsonSchema.Name, 
                _jsonSchemaValidationFactory.Create(code, jsonSchema, parameterType));

            _convertibles!.Add(jsonSchema.Name,
                _convertibleFactory.Create(code, parameterType));

            return this;
        }

        private static System.Reflection.Emit.PropertyBuilder CreateProperty(TypeBuilder builder, string fieldName, Type type)
        {
            var field = builder.DefineField($"_{fieldName}", type, FieldAttributes.Private);
            var parameterName = fieldName.FirstCharToUpper();
            var propertyBuilder = builder.DefineProperty(parameterName, PropertyAttributes.HasDefault, type, null);

            var getProperty = builder.DefineMethod($"get_{parameterName}", s_getSetAttributes, type, Type.EmptyTypes);

            getProperty.GetILGenerator().Return(field);

            // Define the "set" accessor method for CustomerName.
            var setProperty = builder.DefineMethod($"set_{parameterName}", s_getSetAttributes,
                null, new[] {type});

            setProperty.GetILGenerator().Set(field);

            propertyBuilder.SetGetMethod(getProperty);
            propertyBuilder.SetSetMethod(setProperty);
            
            return propertyBuilder;
        }
        
        private (TypeBuilder, System.Reflection.Emit.PropertyBuilder) CreateActionInfo(MethodInfo action, TypeBuilder inputType, Type thingType, string actionName)
        {
            var actionInfo = _module!.DefineType($"{thingType.Name}{action.Name}ActionInfo",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass,
                typeof(ThingActionInformation));
            
            var input = CreateProperty(actionInfo, "input", inputType);
            
            var getProperty = actionInfo.DefineMethod(nameof(ThingActionInformation.GetActionName), 
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, 
                typeof(string), Type.EmptyTypes);

            getProperty.GetILGenerator().Return(actionName);
            
            CreateInternalExecuteAsync(action, actionInfo, inputType, input, thingType);
            actionInfo.CreateType();
            return (actionInfo, input);
            
             static void CreateInternalExecuteAsync(MethodInfo action, TypeBuilder actionInfo, TypeBuilder input, PropertyInfo inputProperty, Type thingType) 
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
        }

        private TypeBuilder CreateActionInfoFactory(Type actionInfo, Type inputType, PropertyInfo inputProperty)
        {
            var actionInfoFactory = _module!.DefineType($"{actionInfo.Name}Convertible",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass,
                null, new []{ typeof(IActionInformationFactory) });

            var createMethod = actionInfoFactory.DefineMethod(nameof(IActionInformationFactory.Convert),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.Standard, 
                typeof(ThingActionInformation), 
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

        /// <inheritdoc /> 
        public Dictionary<string, ActionCollection> Build()
        {
            if (_actions == null || _option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)} before add");
            }
            
            if (_thingType == null || _module == null)
            {
                throw new InvalidOperationException($"ThingType is null, call {nameof(SetThingType)} before add");
            }
            
            if (_input != null)
            {
                _input.CreateType();
                var (actionInfoBuilder, inputProperty) = CreateActionInfo(_action!, _input, _thingType, _name!);
                var factory = CreateActionInfoFactory(actionInfoBuilder, _input, inputProperty);
                
                _actions.Add(_name!, new ActionCollection(
                    new SystemTextJsonInputConvertible(_jsonConvertibles!),
                    new InputJsonSchemaValidation(_jsonSchemaValidations!),
                    new InputConvertible(_convertibles!), 
                    (IActionInformationFactory)Activator.CreateInstance(factory)!));
            }
            
            return _actions;
        }
    }
}
