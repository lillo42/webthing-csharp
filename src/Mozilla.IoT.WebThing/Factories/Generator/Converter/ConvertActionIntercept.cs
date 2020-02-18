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

        public void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInfo)
        {
            if (!_isObjectStart)
            {
                _jsonWriter.StartObject("Actions");
                _isObjectStart = true;
            }

            var name = actionInfo?.Name ?? action.Name;

            _jsonWriter.StartObject(name);

            if (actionInfo != null)
            {
                _jsonWriter.PropertyWithNullableValue("Title", actionInfo.Title);
                _jsonWriter.PropertyWithNullableValue("Description", actionInfo.Description);
                _jsonWriter.PropertyType("@type", actionInfo.Type);
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
                    var jsonType = GetJsonType(parameter.ParameterType);

                    if (jsonType == null)
                    {
                        throw new ArgumentException();
                    }

                    _jsonWriter.PropertyWithValue("Type", jsonType);
                    var parameterActionInfo = parameter.GetCustomAttribute<ThingParameterAttribute>();

                    if (parameterActionInfo != null)
                    {
                        _jsonWriter.PropertyWithNullableValue("Title", parameterActionInfo.Title);
                        _jsonWriter.PropertyWithNullableValue("Description", parameterActionInfo.Description);
                        _jsonWriter.PropertyWithNullableValue("Unit", parameterActionInfo.Unit);
                        if (jsonType == "number" || jsonType == "integer")
                        {
                            _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Minimum), parameter.ParameterType,
                                parameterActionInfo.MinimumValue);
                            _jsonWriter.PropertyNumber(nameof(ThingPropertyAttribute.Maximum), parameter.ParameterType,
                                parameterActionInfo.MaximumValue);
                            _jsonWriter.PropertyWithNullableValue(nameof(ThingPropertyAttribute.MultipleOf),
                                parameterActionInfo.MultipleOfValue);
                        }
                    }

                    _jsonWriter.EndObject();
                }

                _jsonWriter.EndObject();
                _jsonWriter.EndObject();
            }
            else if (actionInfo?.Type != null)
            {
                _jsonWriter.StartObject("Input");
                _jsonWriter.PropertyType("@type", actionInfo.Type);
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
