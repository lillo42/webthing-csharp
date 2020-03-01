using System;
using System.Reflection;
using System.Text.Json;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

using static Mozilla.IoT.WebThing.Factories.Generator.Converter.Helper;

namespace Mozilla.IoT.WebThing.Factories.Generator.Converter
{
    internal class ConverterPropertyIntercept : IPropertyIntercept
    {
        private readonly Utf8JsonWriterILGenerator _jsonWriter;
        private readonly JsonSerializerOptions _options;
        private bool _isObjectStart;

        public ConverterPropertyIntercept(Utf8JsonWriterILGenerator jsonWriter, JsonSerializerOptions options)
        {
            _jsonWriter = jsonWriter ?? throw new ArgumentNullException(nameof(jsonWriter));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Before(Thing thing)
        {

        }

        public void After(Thing thing)
        {
            if (_isObjectStart)
            {
                _jsonWriter.EndObject();
            }
            else
            {
                _jsonWriter.PropertyWithNullValue("Properties");
            }
        }

        public void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute)
        {
            if (!_isObjectStart)
            {
                _jsonWriter.StartObject("Properties");
                _isObjectStart = true;
            }

            var propertyName = _options.GetPropertyName(thingPropertyAttribute?.Name ?? propertyInfo.Name);
            var propertyType = propertyInfo.PropertyType.GetUnderlyingType();
            var jsonType = GetJsonType(propertyType);
            if (jsonType == null)
            {
                return;
            }

            _jsonWriter.StartObject(propertyName);

            if (thingPropertyAttribute != null)
            {
                _jsonWriter.PropertyWithNullableValue(nameof(ThingPropertyAttribute.Title),
                    thingPropertyAttribute.Title);
                _jsonWriter.PropertyWithNullableValue(nameof(ThingPropertyAttribute.Description),
                    thingPropertyAttribute.Description);
                var readOnly = thingPropertyAttribute.IsReadOnly || !propertyInfo.CanWrite || !propertyInfo.SetMethod.IsPublic;
                _jsonWriter.PropertyWithNullableValue("ReadOnly", readOnly);

                if (thingPropertyAttribute.IsWriteOnlyValue.HasValue)
                {
                    _jsonWriter.PropertyWithNullableValue("WriteOnly", thingPropertyAttribute.IsWriteOnlyValue.Value);
                }
                else if(!propertyInfo.CanRead || !propertyInfo.GetMethod.IsPublic)
                {
                    _jsonWriter.PropertyWithNullableValue("WriteOnly", true);
                }
                
                
                _jsonWriter.PropertyWithNullableValue("Type", jsonType.ToString().ToLower());
                _jsonWriter.PropertyEnum("@enum", propertyType, thingPropertyAttribute.Enum);
                _jsonWriter.PropertyWithNullableValue(nameof(ThingPropertyAttribute.Unit), thingPropertyAttribute.Unit);
                _jsonWriter.PropertyType("@type", thingPropertyAttribute.Type);

                if (jsonType == JsonType.Number || jsonType == JsonType.Integer)
                {
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Minimum), propertyType,
                        thingPropertyAttribute.MinimumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Maximum), propertyType,
                        thingPropertyAttribute.MaximumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.ExclusiveMinimum), propertyType,
                        thingPropertyAttribute.ExclusiveMinimumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.ExclusiveMaximum), propertyType,
                        thingPropertyAttribute.ExclusiveMaximumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.MultipleOf), propertyType,
                        thingPropertyAttribute.MultipleOfValue);
                }
                else if (jsonType == JsonType.String)
                {
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.MinimumLength), propertyType,
                        thingPropertyAttribute.MinimumLengthValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.MaximumLength), propertyType,
                        thingPropertyAttribute.MaximumLengthValue);
                    _jsonWriter.PropertyString(nameof(ThingPropertyAttribute.Pattern), propertyType,
                        thingPropertyAttribute.Pattern);
                }
            }
            else
            {
                if (!propertyInfo.CanWrite || !propertyInfo.SetMethod.IsPublic)
                {
                    _jsonWriter.PropertyWithNullableValue("ReadOnly", true);
                }
                
                if (!propertyInfo.CanRead || !propertyInfo.GetMethod.IsPublic)
                {
                    _jsonWriter.PropertyWithNullableValue("WriteOnly", true);
                }
            }
            

            _jsonWriter.StartArray("Links");

            _jsonWriter.StartObject();

            _jsonWriter.PropertyWithValue("href",
                $"/things/{_options.GetPropertyName(thing.Name)}/properties/{propertyName}");

            _jsonWriter.EndObject();
            _jsonWriter.EndArray();

            _jsonWriter.EndObject();
        }

       
    }
}
