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

        public void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? propertyAttribute)
        {
            if (!_isObjectStart)
            {
                _jsonWriter.StartObject("Properties");
                _isObjectStart = true;
            }

            var propertyName = _options.GetPropertyName(propertyAttribute?.Name ?? propertyInfo.Name);
            var propertyType = propertyInfo.PropertyType.GetUnderlyingType();
            var jsonType = GetJsonType(propertyType);
            if (jsonType == null)
            {
                return;
            }

            _jsonWriter.StartObject(propertyName);

            if (propertyAttribute != null)
            {
                _jsonWriter.PropertyWithNullableValue(nameof(ThingPropertyAttribute.Title),
                    propertyAttribute.Title);
                _jsonWriter.PropertyWithNullableValue(nameof(ThingPropertyAttribute.Description),
                    propertyAttribute.Description);
                var readOnly = propertyAttribute.IsReadOnly || !propertyInfo.CanWrite || !propertyInfo.SetMethod.IsPublic;
                _jsonWriter.PropertyWithNullableValue("ReadOnly", readOnly);

                if (propertyAttribute.IsWriteOnlyValue.HasValue)
                {
                    _jsonWriter.PropertyWithNullableValue("WriteOnly", propertyAttribute.IsWriteOnlyValue.Value);
                }
                else if(!propertyInfo.CanRead || !propertyInfo.GetMethod.IsPublic)
                {
                    _jsonWriter.PropertyWithNullableValue("WriteOnly", true);
                }
                
                
                _jsonWriter.PropertyWithNullableValue("Type", jsonType.ToString().ToLower());
                _jsonWriter.PropertyEnum("@enum", propertyType, propertyAttribute.Enum);
                _jsonWriter.PropertyWithNullableValue(nameof(ThingPropertyAttribute.Unit), propertyAttribute.Unit);
                _jsonWriter.PropertyType("@type", propertyAttribute.Type);

                if (jsonType == JsonType.Number || jsonType == JsonType.Integer)
                {
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Minimum), propertyType,
                        propertyAttribute.MinimumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Maximum), propertyType,
                        propertyAttribute.MaximumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.ExclusiveMinimum), propertyType,
                        propertyAttribute.ExclusiveMinimumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.ExclusiveMaximum), propertyType,
                        propertyAttribute.ExclusiveMaximumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.MultipleOf), propertyType,
                        propertyAttribute.MultipleOfValue);
                }
                else if (jsonType == JsonType.String)
                {
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.MinimumLength), propertyType,
                        propertyAttribute.MinimumLengthValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.MaximumLength), propertyType,
                        propertyAttribute.MaximumLengthValue);
                    _jsonWriter.PropertyString(nameof(ThingPropertyAttribute.Pattern), propertyType,
                        propertyAttribute.Pattern);
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
