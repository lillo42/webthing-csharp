using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
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
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass,
                null, new []{ typeof(IProperty<>).MakeGenericType(propertyInfo.PropertyType) });

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
            var generator = constructor.GetILGenerator();
            
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Castclass, thingType);
            generator.Emit(OpCodes.Stfld, field);
            generator.Emit(OpCodes.Ret);
        }
        private static void CreateGetValue(TypeBuilder typeBuilder, PropertyInfo property, FieldBuilder thingField, string propertyName)
        {
            var getValueMethod = typeBuilder.DefineMethod(nameof(IProperty.GetValue), 
                MethodAttributes.Public | MethodAttributes.Final |  MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                property.PropertyType, Type.EmptyTypes);

            var generator = getValueMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, thingField);
            generator.EmitCall(OpCodes.Callvirt, property.GetMethod, null);
            generator.Emit(OpCodes.Ret);
            
            var getMethod = typeBuilder.DefineMethod($"get_{propertyName}", 
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                property.PropertyType, Type.EmptyTypes);
            
            generator = getMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, thingField);
            generator.EmitCall(OpCodes.Callvirt, property.GetMethod, null);
            generator.Emit(OpCodes.Ret);

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

            var generator = setValue.GetILGenerator();

            if (!property.CanWrite || !property.SetMethod.IsPublic ||
                (propertyValidation != null && propertyValidation.IsReadOnly))
            {
                generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.ReadOnly);
                generator.Emit(OpCodes.Ret);
                return;
            }

            var propertyType = property.PropertyType.GetUnderlyingType();
            var jsonElement = generator.DeclareLocal(typeof(JsonElement));
            var local = generator.DeclareLocal(propertyType);
            var nullable = local;
            if (property.PropertyType.IsNullable())
            {
                nullable = generator.DeclareLocal(property.PropertyType);
            }

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Ldloca_S, jsonElement.LocalIndex);

            var getter = new JsonElementReaderILGenerator(generator);
            var validator = ToValidation(propertyValidation);

            var next = generator.DefineLabel();
            if (propertyType == typeof(string))
            {
                getter.GetValueKind();
                generator.Emit(OpCodes.Ldc_I4_S, (int)JsonValueKind.Null);
                generator.Emit(OpCodes.Bne_Un_S, next);

                if (validator.HasValidation)
                {
                    generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.InvalidValue);
                    generator.Emit(OpCodes.Ret);
                }
                else
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, thingField);
                    generator.Emit(OpCodes.Ldnull);
                    generator.EmitCall(OpCodes.Callvirt, property.SetMethod, null);

                    generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.Ok);
                    generator.Emit(OpCodes.Ret);
                }

                generator.MarkLabel(next);
                next = generator.DefineLabel();

                generator.Emit(OpCodes.Ldloca_S, jsonElement.LocalIndex);
                getter.GetValueKind();
                generator.Emit(OpCodes.Ldc_I4_S, (int)JsonValueKind.String);
                generator.Emit(OpCodes.Beq_S, next);

                generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.InvalidValue);
                generator.Emit(OpCodes.Ret);

                generator.MarkLabel(next);

                generator.Emit(OpCodes.Ldloca_S, jsonElement.LocalIndex);
                getter.Get(propertyType);
                generator.Emit(OpCodes.Stloc_S, local.LocalIndex);
            }
            else if (propertyType == typeof(bool))
            {
                if (property.PropertyType.IsNullable())
                {
                    getter.GetValueKind();
                    generator.Emit(OpCodes.Ldc_I4_S, (int)JsonValueKind.Null);
                    generator.Emit(OpCodes.Bne_Un_S, next);

                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, thingField);
                    generator.Emit(OpCodes.Ldloca_S, nullable.LocalIndex);
                    generator.Emit(OpCodes.Initobj, nullable.LocalType);
                    generator.Emit(OpCodes.Ldloc_S, nullable.LocalIndex);
                    generator.EmitCall(OpCodes.Callvirt, property.SetMethod, null);

                    generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.Ok);
                    generator.Emit(OpCodes.Ret);

                    generator.MarkLabel(next);
                    next = generator.DefineLabel();

                    generator.Emit(OpCodes.Ldloca_S, jsonElement.LocalIndex);
                }

                getter.GetValueKind();
                generator.Emit(OpCodes.Ldc_I4_S, (byte)JsonValueKind.True);
                generator.Emit(OpCodes.Beq_S, next);

                generator.Emit(OpCodes.Ldloca_S, jsonElement.LocalIndex);
                getter.GetValueKind();
                generator.Emit(OpCodes.Ldc_I4_S, (byte)JsonValueKind.False);
                generator.Emit(OpCodes.Beq_S, next);

                generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.InvalidValue);
                generator.Emit(OpCodes.Ret);

                generator.MarkLabel(next);

                generator.Emit(OpCodes.Ldloca_S, jsonElement.LocalIndex);
                getter.Get(propertyType);
                generator.Emit(OpCodes.Stloc_S, local.LocalIndex);
            }
            else
            {
                if (property.PropertyType.IsNullable())
                {
                    getter.GetValueKind();
                    generator.Emit(OpCodes.Ldc_I4_S, (int)JsonValueKind.Null);
                    generator.Emit(OpCodes.Bne_Un_S, next);

                    if (validator.HasValidation)
                    {
                        generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.InvalidValue);
                        generator.Emit(OpCodes.Ret);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, thingField);
                        generator.Emit(OpCodes.Ldloca_S, nullable.LocalIndex);
                        generator.Emit(OpCodes.Initobj, nullable.LocalType);
                        generator.Emit(OpCodes.Ldloc_S, nullable.LocalIndex);
                        generator.EmitCall(OpCodes.Callvirt, property.SetMethod, null);

                        generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.Ok);
                        generator.Emit(OpCodes.Ret);
                    }

                    generator.MarkLabel(next);
                    next = generator.DefineLabel();

                    generator.Emit(OpCodes.Ldloca_S, jsonElement.LocalIndex);
                }

                var isDate = propertyType == typeof(DateTime) || propertyType == typeof(DateTimeOffset);
                getter.GetValueKind();
                generator.Emit(OpCodes.Ldc_I4_S, isDate ? (byte)JsonValueKind.String : (byte)JsonValueKind.Number);
                generator.Emit(OpCodes.Beq_S, next);

                generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.InvalidValue);
                generator.Emit(OpCodes.Ret);

                generator.MarkLabel(next);
                next = generator.DefineLabel();

                generator.Emit(OpCodes.Ldloca_S, jsonElement.LocalIndex);
                generator.Emit(OpCodes.Ldloca_S, local.LocalIndex);
                getter.TryGet(propertyType);
                generator.Emit(OpCodes.Brtrue_S, next);

                generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.InvalidValue);
                generator.Emit(OpCodes.Ret);

                generator.MarkLabel(next);
            }

            if (validator.HasValidation)
            {
                Label? validationMark = null;
                var validationGeneration = new ValidationGeneration(generator, typeBuilder);
                validationGeneration.AddValidation(propertyType, validator,
                    () => generator.Emit(OpCodes.Ldloc_S, local.LocalIndex), () =>
                    {
                        generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.InvalidValue);
                        generator.Emit(OpCodes.Ret);
                    }, ref validationMark);

                if (validationMark.HasValue)
                {
                    generator.MarkLabel(validationMark.Value);
                }
            }

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, thingField);
            generator.Emit(OpCodes.Ldloc_S, local.LocalIndex);
            
            if (property.PropertyType.IsNullable())
            {
                var constructor = nullable.LocalType.GetConstructors().Last();
                generator.Emit(OpCodes.Newobj, constructor);
            }
            
            generator.EmitCall(OpCodes.Callvirt, property.SetMethod, null);
            generator.Emit(OpCodes.Ldc_I4_S, (int)SetPropertyResult.Ok);
            generator.Emit(OpCodes.Ret);

            static Validation ToValidation(ThingPropertyAttribute propertyValidation)
                => new Validation(propertyValidation?.MinimumValue, propertyValidation?.MaximumValue,
                    propertyValidation?.ExclusiveMinimumValue, propertyValidation?.ExclusiveMaximumValue,
                    propertyValidation?.MultipleOfValue,
                    propertyValidation?.MinimumLengthValue, propertyValidation?.MaximumLengthValue,
                    propertyValidation?.Pattern);
        }
    }
}
