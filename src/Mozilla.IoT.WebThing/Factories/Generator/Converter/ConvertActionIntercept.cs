using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

using static Mozilla.IoT.WebThing.Factories.Generator.Converter.Helper;

namespace Mozilla.IoT.WebThing.Factories.Generator.Converter
{
    internal class ConvertActionIntercept : IActionIntercept
    {
        private readonly Utf8JsonWriterILGenerator _jsonWriter;
        private readonly JsonSerializerOptions _options;
        private bool _isObjectStart;

        public ConvertActionIntercept(Utf8JsonWriterILGenerator jsonWriter, JsonSerializerOptions options)
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
                _jsonWriter.PropertyWithNullValue("Actions");
            }
        }

        public void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInformation)
        {
            if (!_isObjectStart)
            {
                _jsonWriter.StartObject("Actions");
                _isObjectStart = true;
            }

            var name = actionInformation?.Name ?? action.Name;

            _jsonWriter.StartObject(name);

            if (actionInformation != null)
            {
                _jsonWriter.PropertyWithNullableValue("Title", actionInformation.Title);
                _jsonWriter.PropertyWithNullableValue("Description", actionInformation.Description);
                _jsonWriter.PropertyType("@type", actionInformation.Type);
            }

            var parameters = action.GetParameters();

            if (parameters.Length > 0)
            {
                _jsonWriter.StartObject("Input");

                _jsonWriter.PropertyWithValue("Type", "object");

                _jsonWriter.StartObject("Properties");
                foreach (var parameter in parameters)
                {
                    if (parameter.GetCustomAttribute<FromServicesAttribute>() != null
                        || parameter.ParameterType == typeof(CancellationToken))
                    {
                        continue;
                    }

                    _jsonWriter.StartObject(parameter.Name!);
                    var parameterType = parameter.ParameterType.GetUnderlyingType();
                    var jsonType = GetJsonType(parameterType);

                    if (jsonType == null)
                    {
                        throw new ArgumentException();
                    }

                    _jsonWriter.PropertyWithValue("Type", jsonType.ToString()!.ToLower());
                    var parameterActionInfo = parameter.GetCustomAttribute<ThingParameterAttribute>();

                    if (parameterActionInfo != null)
                    {
                        _jsonWriter.PropertyWithNullableValue("Title", parameterActionInfo.Title);
                        _jsonWriter.PropertyWithNullableValue("Description", parameterActionInfo.Description);
                        _jsonWriter.PropertyWithNullableValue("Unit", parameterActionInfo.Unit);
                        
                        var enums = parameterActionInfo.Enum;
                        if (parameterType.IsEnum && (enums == null || enums.Length == 0))
                        {
                            var values = parameterType.GetEnumValues();
                            enums = new object[values.Length];
                            values.CopyTo(enums, 0);
                        }
                        _jsonWriter.PropertyEnum("@enum", parameterType, enums);
                        
                        if (jsonType == JsonType.Number || jsonType == JsonType.Integer)
                        {
                            _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Minimum), parameterType,
                                parameterActionInfo.MinimumValue);
                            _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Maximum), parameterType,
                                parameterActionInfo.MaximumValue);
                            _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.ExclusiveMinimum), parameterType,
                                parameterActionInfo.ExclusiveMinimumValue);
                            _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.ExclusiveMaximum), parameterType,
                                parameterActionInfo.ExclusiveMaximumValue);
                            _jsonWriter.PropertyWithNullableValue(nameof(ThingPropertyAttribute.MultipleOf),
                                parameterActionInfo.MultipleOfValue);
                        }
                        else if (jsonType == JsonType.String)
                        {
                            _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.MinimumLength), parameterType,
                                parameterActionInfo.MinimumLengthValue);
                            _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.MaximumLength), parameterType,
                                parameterActionInfo.MaximumLengthValue);
                            _jsonWriter.PropertyString(nameof(ThingPropertyAttribute.Pattern),
                                parameterActionInfo.Pattern);
                        }
                    }

                    _jsonWriter.EndObject();
                }

                _jsonWriter.EndObject();
                _jsonWriter.EndObject();
            }
            else if (actionInformation?.Type != null)
            {
                _jsonWriter.StartObject("Input");
                _jsonWriter.PropertyType("@type", actionInformation.Type);
                _jsonWriter.EndObject();
            }

            _jsonWriter.StartArray("Links");
            _jsonWriter.StartObject();
            _jsonWriter.PropertyWithValue("href",
                $"/things/{_options.GetPropertyName(thing.Name)}/actions/{_options.GetPropertyName(name)}");
            _jsonWriter.EndObject();
            _jsonWriter.EndArray();
            _jsonWriter.EndObject();
        }
    }
}
