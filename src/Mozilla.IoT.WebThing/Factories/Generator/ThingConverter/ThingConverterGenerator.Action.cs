using System;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    internal sealed partial class ThingConverterGenerator
    {
        private void GenerateActions(Thing thing, Type thingType)
        {
            var actionsTypes = thingType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !x.IsSpecialName
                    && x.Name != nameof(Equals) && x.Name != nameof(GetType) 
                    && x.Name != nameof(GetHashCode) && x.Name != nameof(ToString))
                .ToArray();
            
            if (actionsTypes.Length == 0)
            {
                PropertyWithNullValue("Actions");
                return;
            }
            
            StartObject("Actions");

            foreach (var actionType in actionsTypes)
            {
                var information = actionType.GetCustomAttribute<ThingActionAttribute>();
                if (information != null && information.Ignore)
                {
                    continue;
                }

                var name = information?.Name ?? actionType.Name;
                StartObject(name);

                if (information != null)
                {
                    PropertyWithNullableValue("Title", information.Title);
                    PropertyWithNullableValue("Description", information.Description);
                    PropertyType("@type", information?.Type);
                }

                var parameters = actionType.GetParameters();

                if (parameters.Length > 0)
                {
                    StartObject("Input");
                    
                    PropertyWithValue("Type", "object");
                    
                    StartObject("Properties");
                    foreach (var parameter in parameters)
                    {
                        StartObject(parameter.Name);
                        var jsonType = GetJsonType(parameter.ParameterType);
                        
                        if (jsonType == null)
                        {
                            throw new ArgumentException();
                        }
                        
                        PropertyWithValue("Type", jsonType);
                        var parameterInformation = parameter.GetCustomAttribute<ThingParameterAttribute>();

                        if (parameterInformation != null)
                        {
                            PropertyWithNullableValue("Title", parameterInformation.Title);
                            PropertyWithNullableValue("Description", parameterInformation.Description);
                            PropertyWithNullableValue("Unit", parameterInformation.Unit);
                            if (jsonType == "number" || jsonType == "integer")
                            {
                                PropertyNumber(nameof(ThingPropertyAttribute.Minimum), parameter.ParameterType, parameterInformation.MinimumValue);
                                PropertyNumber(nameof(ThingPropertyAttribute.Maximum), parameter.ParameterType, parameterInformation.MaximumValue);
                                PropertyWithNullableValue(nameof(ThingPropertyAttribute.MultipleOf), parameterInformation.MultipleOfValue);
                            }
                        }
                        
                        EndObject();
                    }
                        
                    EndObject();
                    EndObject();
                }
                else if(information?.Type != null)
                {
                    StartObject("Input");
                    
                    PropertyType("@type", information.Type);
                    
                    EndObject();
                }
                
                StartArray("Links");
                StartObject();
                PropertyWithValue("href", $"/things/{thing.Name}/actions/{GetPropertyName(name)}");
                EndObject();
                EndArray();

                EndObject();
            }

            EndObject();
        }
    }
}
