using System;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class AttributeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static JsonSchema ToJsonSchema(this ThingPropertyAttribute attribute, PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            var propertyName = attribute?.Name ?? property.Name;

            bool isNullable;
            bool? isReadOnly = null;

            if (attribute is IJsonSchema jsonSchema)
            {
                if (jsonSchema.IsNullable.HasValue)
                {
                    isNullable = jsonSchema.IsNullable.Value;
                }
                else if(jsonSchema.Enum != null)
                {
                    isNullable = jsonSchema.Enum.Contains(null!);
                }
                else
                {
                    isNullable = !propertyType.IsByRef || property.PropertyType.IsNullable();  
                }

                if (jsonSchema.IsReadOnly.HasValue)
                {
                    isReadOnly = jsonSchema.IsReadOnly.Value;
                }
                else if (!property.CanWrite || !property.SetMethod!.IsPublic)
                {
                    isReadOnly = true;
                }
            }
            else
            {
                isNullable = !propertyType.IsByRef || property.PropertyType.IsNullable();
                    
                if (!property.CanWrite || !property.SetMethod!.IsPublic)
                {
                    isReadOnly = true;
                }
            }
            
            return new JsonSchema(attribute, GetEnums(propertyType, attribute?.Enum),
                propertyType.ToJsonType(), propertyName, isNullable, isReadOnly);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static JsonSchema ToJsonSchema(this ThingParameterAttribute attribute, ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            var name = attribute?.Name ?? parameter.Name;

            bool isNullable;
            if (attribute is IJsonSchema jsonSchema)
            {
                if (jsonSchema.IsNullable.HasValue)
                {
                    isNullable = jsonSchema.IsNullable.Value;
                }
                else if(jsonSchema.Enum != null)
                {
                    isNullable = jsonSchema.Enum.Contains(null!);
                }
                else
                {
                    isNullable = !parameterType.IsByRef || parameterType.IsNullable();  
                }
            }
            else
            {
                isNullable = !parameterType.IsByRef || parameterType.IsNullable();
            }

            return new JsonSchema(attribute,  GetEnums(parameterType, attribute?.Enum), 
                parameterType.ToJsonType(), name!, isNullable);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static JsonSchema ToJsonSchema(this ThingActionAttribute attribute, MethodInfo method)
        {
            var name = attribute?.Name ?? method.Name;
            return new JsonSchema(attribute, null, JsonType.Object, name, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static JsonSchema ToJsonSchema(this ThingEventAttribute attribute, EventInfo @event)
        {
            var parameterType = @event.EventHandlerType!.GetGenericArguments()[0];
            var name = attribute?.Name ?? @event.Name;

            bool isNullable;
            if (attribute is IJsonSchema jsonSchema)
            {
                if (jsonSchema.IsNullable.HasValue)
                {
                    isNullable = jsonSchema.IsNullable.Value;
                }
                else if(jsonSchema.Enum != null)
                {
                    isNullable = jsonSchema.Enum.Contains(null!);
                }
                else
                {
                    isNullable = !parameterType.IsByRef || parameterType.IsNullable();  
                }
            }
            else
            {
                isNullable = !parameterType.IsByRef || parameterType.IsNullable();
            }

            return new JsonSchema(attribute,  GetEnums(parameterType, attribute?.Enum), 
                parameterType.ToJsonType(), name, isNullable);
        }


        private static object[]? GetEnums(Type type, object[]? values)
        {
            type = type.GetUnderlyingType();
            if (type.IsEnum)
            {
                var enumValues = type.GetEnumNames();
                var result = new object[enumValues.Length];
                Array.Copy(enumValues, 0, result, 0, result.Length);
                return result;
            }

            return values;
        }
    }
}
