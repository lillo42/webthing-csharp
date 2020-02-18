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
            var propertyType = propertyInfo.PropertyType;
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
                _jsonWriter.PropertyWithNullableValue("Type", jsonType);
                _jsonWriter.PropertyEnum("@enum", propertyType, thingPropertyAttribute.Enum);
                _jsonWriter.PropertyWithNullableValue(nameof(ThingPropertyAttribute.Unit), thingPropertyAttribute.Unit);
                _jsonWriter.PropertyType("@type", thingPropertyAttribute.Type);

                if (jsonType == "number" || jsonType == "integer")
                {
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Minimum), propertyType,
                        thingPropertyAttribute.MinimumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Maximum), propertyType,
                        thingPropertyAttribute.MaximumValue);
                    _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.MultipleOf), propertyType,
                        thingPropertyAttribute.MultipleOfValue);
                }
            }
            else
            {
                if (!propertyInfo.CanWrite || !propertyInfo.SetMethod.IsPublic)
                {
                    _jsonWriter.PropertyWithNullableValue("ReadOnly", true);
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
