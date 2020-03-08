using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Properties
{
    public class PropertiesIntercept : IPropertyIntercept
    {
        private readonly ThingOption _option;
        private readonly ModuleBuilder _moduleBuilder;

        public Dictionary<string, IProperty> Properties { get; }
        
        public PropertiesIntercept(ThingOption option, ModuleBuilder moduleBuilder)
        {
            _option = option ?? throw new ArgumentNullException(nameof(option));
            _moduleBuilder = moduleBuilder ?? throw new ArgumentNullException(nameof(moduleBuilder));
            Properties = option.IgnoreCase ? new Dictionary<string, IProperty>(StringComparer.InvariantCultureIgnoreCase) 
                : new Dictionary<string, IProperty>();
        }

        
        public void Before(Thing thing)
        {
            
        }
        
        public void After(Thing thing)
        {
            
        }
        
        public void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute)
        {
            var thingType = thing.GetType();
            var propertyName = thingPropertyAttribute?.Name ?? propertyInfo.Name;
            var typeBuilder = _moduleBuilder.DefineType($"{propertyInfo.Name}{thingType.Name}PropertyThing",
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.BeforeFieldInit |  TypeAttributes.AnsiClass,
                typeof(ValueType), new []{ typeof(IProperty<>).MakeGenericType(propertyInfo.PropertyType) });

            var isReadOnly = typeof(IsReadOnlyAttribute).GetConstructors()[0];
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(isReadOnly, new object?[0]));
            
            var thingField = typeBuilder.DefineField("_thing", thing.GetType(), FieldAttributes.Private | FieldAttributes.InitOnly);
            
            CreateConstructor(typeBuilder, thingField, thingType);
            CreateGetValue(typeBuilder, propertyInfo, thingField, propertyName);
            CreateSetValidation(typeBuilder, propertyInfo, thingPropertyAttribute, thingField);

            var propertyType = typeBuilder.CreateType();
            Properties.Add(_option.PropertyNamingPolicy.ConvertName(propertyName), 
                (IProperty)Activator.CreateInstance(propertyType, thing));
        }

        private static void CreateConstructor(TypeBuilder typeBuilder, FieldBuilder field, Type thingType)
        {
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, 
                CallingConventions.Standard, new[] {typeof(Thing)});
            IlFactory.Constructor(constructor.GetILGenerator(), field, thingType);
        }
        private static void CreateGetValue(TypeBuilder typeBuilder, PropertyInfo property, FieldBuilder thingField, string propertyName)
        {
            var getValueMethod = typeBuilder.DefineMethod(nameof(IProperty.GetValue), 
                MethodAttributes.Public | MethodAttributes.Final |  MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                property.PropertyType, Type.EmptyTypes);

            new IlFactory(getValueMethod.GetILGenerator()).GetProperty(thingField, property.GetMethod);

            var getMethod = typeBuilder.DefineMethod($"get_{propertyName}", 
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                property.PropertyType, Type.EmptyTypes);
            
            new IlFactory(getMethod.GetILGenerator()).GetProperty(thingField, property.GetMethod);

            var getProperty = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, property.PropertyType, null);
            getProperty.SetGetMethod(getMethod);
        }
        private static void CreateSetValidation(TypeBuilder typeBuilder, PropertyInfo property,
            ThingPropertyAttribute? propertyValidation, FieldBuilder thingField)
        {
            var setValue = typeBuilder.DefineMethod(nameof(IProperty.SetValue),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                MethodAttributes.Virtual,
                typeof(SetPropertyResult), new[] {typeof(JsonElement)});

            FieldBuilder? regex = null;
            var validator = ToValidation(propertyValidation);
            if (validator.Pattern != null)
            {
                regex = typeBuilder.DefineField("_regex", typeof(Regex), FieldAttributes.Private | FieldAttributes.InitOnly | FieldAttributes.Static);
                var staticConstructor = typeBuilder.DefineTypeInitializer();
                var staticConstructorFactory = new IlFactory(staticConstructor.GetILGenerator());
                staticConstructorFactory.InitializerRegex(regex, validator.Pattern);
            }
            
            var generator = setValue.GetILGenerator();
            var factory = new IlFactory(generator);

            if (!property.CanWrite || !property.SetMethod.IsPublic || (propertyValidation != null && propertyValidation.IsReadOnly))
            {
                factory.Return((int)SetPropertyResult.ReadOnly);
                return;
            }

            var propertyType = property.PropertyType.GetUnderlyingType();
            var jsonElement = factory.CreateLocalField(typeof(JsonElement));
            var local = factory.CreateLocalField(propertyType);
            var nullable = local;
            
            if (property.PropertyType.IsNullable())
            {
                nullable = factory.CreateLocalField(property.PropertyType);
            }

            factory.SetArgToLocal(jsonElement);

            if (propertyType == typeof(string))
            {
                factory.IfIsEquals(jsonElement, JsonElementMethods.ValueKind, (int)JsonValueKind.Null);

                if (validator.HasValidation && !validator.HasNullOnEnum)
                {
                    factory.Return((int)SetPropertyResult.InvalidValue);
                }
                else
                {
                    factory.SetNullValue(thingField, property.SetMethod);
                    factory.Return((int)SetPropertyResult.Ok);
                }
                
                factory.EndIf();
                
                factory.IfIsDifferent(jsonElement, JsonElementMethods.ValueKind ,(int)JsonValueKind.String);
                factory.Return((int)SetPropertyResult.InvalidValue);
                factory.EndIf();

                factory.SetLocal(jsonElement, JsonElementMethods.GetValue(propertyType), local);
            }
            else if (propertyType == typeof(bool))
            {
                if (property.PropertyType.IsNullable())
                {
                    factory.IfIsEquals(jsonElement, JsonElementMethods.ValueKind, (int)JsonValueKind.Null);
                    
                    factory.SetNullValue(thingField, property.SetMethod, nullable);
                    factory.Return((int)SetPropertyResult.Ok);
                    
                    factory.EndIf();
                }
                
                factory.IfIsDifferent(jsonElement, JsonElementMethods.ValueKind,(int)JsonValueKind.True, (int)JsonValueKind.False );
                factory.Return((int)SetPropertyResult.InvalidValue);
                factory.EndIf();
                
                factory.SetLocal(jsonElement, JsonElementMethods.GetValue(propertyType), local);
            }
            else
            {
                if (property.PropertyType.IsNullable())
                {
                    factory.IfIsEquals(jsonElement, JsonElementMethods.ValueKind, (int)JsonValueKind.Null);
                    
                    if (validator.HasValidation && !validator.HasNullOnEnum)
                    {
                        factory.Return((int)SetPropertyResult.InvalidValue);
                    }
                    else
                    {
                        factory.SetNullValue(thingField, property.SetMethod, nullable);
                        factory.Return((int)SetPropertyResult.Ok);
                    }

                    factory.EndIf();
                }

                var valueKind = propertyType == typeof(DateTime) || propertyType == typeof(DateTimeOffset) ? 
                    (int)JsonValueKind.String : (int)JsonValueKind.Number;
                
                factory.IfIsDifferent(jsonElement, JsonElementMethods.ValueKind, valueKind);
                factory.Return((int)SetPropertyResult.InvalidValue);
                factory.EndIf();
                
                factory.IfTryGetIsFalse(jsonElement, local, JsonElementMethods.TryGetValue(propertyType));
                factory.Return((int)SetPropertyResult.InvalidValue);
                factory.EndIf();
            }

            if (validator.HasValidation)
            {
                ValidationGeneration.AddValidation(factory, validator, local, (int)SetPropertyResult.InvalidValue, regex);
            }

            factory.SetValue(local, thingField, property.SetMethod);
            factory.Return((int)SetPropertyResult.Ok);
            
            static Validation ToValidation(ThingPropertyAttribute propertyValidation)
                => new Validation(propertyValidation?.MinimumValue, propertyValidation?.MaximumValue,
                    propertyValidation?.ExclusiveMinimumValue, propertyValidation?.ExclusiveMaximumValue,
                    propertyValidation?.MultipleOfValue,
                    propertyValidation?.MinimumLengthValue, propertyValidation?.MaximumLengthValue,
                    propertyValidation?.Pattern, propertyValidation?.Enum);
        }
    }
}
