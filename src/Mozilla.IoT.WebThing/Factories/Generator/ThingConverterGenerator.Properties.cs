using System;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    internal sealed partial class ThingConverterGenerator
    {
        private void GenerateProperties(Thing thing, Type type, bool shouldInsertLink)
        {
            StartObject("Properties");

            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !IsThingProperty(x.Name)))
            {
                var propertyType = propertyInfo.PropertyType;
                var jsonType = GetJsonType(propertyType);
                if (jsonType == null)
                {
                    continue;
                }
                
                var ignore = propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>();

                if (ignore != null)
                {
                    continue;
                }

                var propertyNameAttribute = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
                var propertyName = propertyNameAttribute?.Name ?? propertyInfo.Name;
                StartObject(propertyName);
                
                var propertyInformation = propertyInfo.GetCustomAttribute<JsonPropertyInformationAttribute>();

                if (propertyInformation != null)
                {
                    PropertyWithNullableValue(nameof(JsonPropertyInformationAttribute.Title), propertyInformation.Title);
                    PropertyWithNullableValue(nameof(JsonPropertyInformationAttribute.Description), propertyInformation.Description);
                    var isReadOnly = propertyInformation?.IsReadOnly ?? !propertyInfo.CanWrite;
                    PropertyWithNullableValue("ReadOnly", isReadOnly);
                    PropertyEnum("@enum", propertyType, propertyInformation.Enum);
                    PropertyType("@type", propertyInformation.Type);
                    
                    if (jsonType == "number" || jsonType == "integer")
                    {
                        PropertyNumber(nameof(JsonPropertyInformationAttribute.Minimum), propertyType, propertyInformation.Minimum);
                        PropertyNumber(nameof(JsonPropertyInformationAttribute.Maximum), propertyType, propertyInformation.Maximum);
                        PropertyWithNullableValue(nameof(JsonPropertyInformationAttribute.MultipleOf), propertyInformation.MultipleOf);
                    }
                }

                PropertyWithNullableValue("ReadOnly", !propertyInfo.CanWrite);
                PropertyWithNullableValue("Type", jsonType);

                if (shouldInsertLink)
                {
                    StartArray("Links");

                    StartObject();
                    
                    PropertyWithValue( "href", $"/things/{thing.Name}/properties/{propertyName}");
                    
                    EndObject();
                    EndArray();
                }
                
                EndObject();
            }
            
            EndObject();

            void PropertyType(string propertyName, string[]? types)
            {
                if (types == null)
                {
                    PropertyWithNullValue(propertyName);
                }
                else if (types.Length == 1)
                {
                    PropertyWithValue(propertyName, types[0]);
                }
                else
                {
                    StartArray(propertyName);

                    foreach (var value in types)
                    {
                        if (value == null)
                        {
                            NullValue();
                        }
                        else
                        {
                            Value(value);
                        }
                    }
                    
                    EndArray();
                }
            }

            void PropertyNumber(string propertyName, Type propertyType, float? value)
            {
                if (value == null)
                {
                    PropertyWithNullValue(propertyName);
                    return;
                }

                if (propertyType == typeof(int)
                    || propertyType == typeof(byte)
                    || propertyType == typeof(short)
                    || propertyType == typeof(ushort))
                {
                    PropertyWithValue(propertyName, (int)value);
                }
                else if (propertyType == typeof(uint))
                {
                    PropertyWithValue(propertyName, (uint)value);
                }
                else if (propertyType == typeof(long))
                {
                    PropertyWithValue(propertyName, (long)value);
                }
                else if (propertyType == typeof(ulong))
                {
                    PropertyWithValue(propertyName, (ulong)value);
                }
                else if (propertyType == typeof(double))
                {
                    PropertyWithValue(propertyName, (double)value);
                }
                else
                {
                    PropertyWithValue(propertyName, (float)value);
                }
            }

            void PropertyEnum(string propertyName, Type propertyType, object[]? @enums)
            {
                if (enums == null)
                {
                    PropertyWithNullValue(propertyName);
                    return;
                }
                
                StartArray(propertyName);

                if (propertyType == typeof(string))
                {
                    foreach (var @enum in enums)
                    {
                        if (@enum == null)
                        {
                            NullValue();
                        }
                        else
                        {
                            Value((string)@enum);
                        }
                    }
                }
                else if (propertyType == typeof(bool))
                {
                    foreach (var @enum in enums)
                    {
                        if (@enum == null)
                        {
                            NullValue();
                        }
                        else
                        {
                            Value((bool)@enum);
                        }
                    }
                }
                else if (propertyType == typeof(int)
                         || propertyType == typeof(byte)
                         || propertyType == typeof(short)
                         || propertyType == typeof(ushort))
                {
                    foreach (var @enum in enums)
                    {
                        if (@enum == null)
                        {
                            NullValue();
                        }
                        else
                        {
                            Value((int)@enum);
                        }
                    }
                }
                else if (propertyType == typeof(uint))
                {
                    foreach (var @enum in enums)
                    {
                        if (@enum == null)
                        {
                            NullValue();
                        }
                        else
                        {
                            Value((uint)@enum);
                        }
                    }
                }
                else if (propertyType == typeof(long))
                {
                    foreach (var @enum in enums)
                    {
                        if (@enum == null)
                        {
                            NullValue();
                        }
                        else
                        {
                            Value((long)@enum);
                        }
                    }
                }
                else if (propertyType == typeof(ulong))
                {
                    foreach (var @enum in enums)
                    {
                        if (@enum == null)
                        {
                            NullValue();
                        }
                        else
                        {
                            Value((ulong)@enum);
                        }
                    }
                }
                else if (propertyType == typeof(double))
                {
                    foreach (var @enum in enums)
                    {
                        if (@enum == null)
                        {
                            NullValue();
                        }
                        else
                        {
                            Value((double)@enum);
                        }
                    }
                }
                else if (propertyType == typeof(float))
                {
                    foreach (var @enum in enums)
                    {
                        if (@enum == null)
                        {
                            NullValue();
                        }
                        else
                        {
                            Value((float)@enum);
                        }
                    }
                }
                
                EndArray();
            }
        }

        private static bool IsThingProperty(string name)
            => name == nameof(Thing.Context)
               || name == nameof(Thing.Name)
               || name == nameof(Thing.Description)
               || name == nameof(Thing.Title)
               || name == nameof(Thing.Title);
    }
}
