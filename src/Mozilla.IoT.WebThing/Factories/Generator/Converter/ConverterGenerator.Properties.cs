using System;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator.Converter
{
    internal sealed partial class ConverterGenerator
    {
        private void GenerateProperties(Thing thing, Type thingType)
        {
            var properties = thingType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !IsThingProperty(x.Name)).ToArray();

            if (properties.Length == 0)
            {
                PropertyWithNullValue("Properties");
                return;
            }
            
            StartObject("Properties");

            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType;
                var jsonType = GetJsonType(propertyType);
                if (jsonType == null)
                {
                    continue;
                }
                
                var information = propertyInfo.GetCustomAttribute<ThingPropertyAttribute>();

                if (information != null && information.Ignore)
                {
                    continue;
                }
                
                var propertyName =  information?.Name ?? propertyInfo.Name;
                StartObject(propertyName);

                if (information != null)
                {
                    PropertyWithNullableValue(nameof(ThingPropertyAttribute.Title), information.Title);
                    PropertyWithNullableValue(nameof(ThingPropertyAttribute.Description), information.Description);
                    var isReadOnly = information?.IsReadOnly ?? !propertyInfo.CanWrite;
                    PropertyWithNullableValue("ReadOnly", isReadOnly);
                    PropertyEnum("@enum", propertyType, information.Enum);
                    PropertyWithNullableValue("Unit", information.Unit);
                    PropertyType("@type", information.Type);
                    
                    if (jsonType == "number" || jsonType == "integer")
                    {
                        PropertyNumber(nameof(ThingPropertyAttribute.Minimum), propertyType, information.MinimumValue);
                        PropertyNumber(nameof(ThingPropertyAttribute.Maximum), propertyType, information.MaximumValue);
                        PropertyWithNullableValue(nameof(ThingPropertyAttribute.MultipleOf), information.MultipleOfValue);
                    }
                }

                PropertyWithNullableValue("ReadOnly", !propertyInfo.CanWrite);
                PropertyWithNullableValue("Type", jsonType);
                
                StartArray("Links");

                StartObject();
                
                PropertyWithValue( "href", $"/things/{thing.Name}/properties/{GetPropertyName(propertyName)}");
                
                EndObject();
                EndArray();

                EndObject();
            }
            
            EndObject();
        }

        private static bool IsThingProperty(string name)
            => name == nameof(Thing.Context)
               || name == nameof(Thing.Name)
               || name == nameof(Thing.Description)
               || name == nameof(Thing.Title)
               || name == nameof(Thing.Type);
    }
}
