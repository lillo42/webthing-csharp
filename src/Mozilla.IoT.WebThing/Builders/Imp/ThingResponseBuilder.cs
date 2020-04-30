using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <inheritdoc />
    public class ThingResponseBuilder : IThingResponseBuilder
    {
        private Thing? _thing;
        private ThingOption? _option;
        private readonly Dictionary<string, object> _events = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _actions = new Dictionary<string, object>();

        private Dictionary<string, object?>? _parameters;
        private string _thingName = string.Empty;
        
        private const string s_items = "items";
        private const string s_type = "type";

        private const string s_enum = "enum";
        private const string s_readOnly = "readOnly";
        private const string s_writeOnly = "writeOnly";
        private const string s_deprecated = "deprecated";
        
        private const string s_pattern = "pattern";
        private const string s_minLength = "minLength";
        private const string s_maxLength = "maxLength";

        private const string s_min = "minimum";
        private const string s_max = "maximum";
        private const string s_minExclusive = "exclusiveMinimum";
        private const string s_maxExclusive = "exclusiveMaximum";
        private const string s_multipleOf = "multipleOf";

        private const string s_minItems = "minItems";
        private const string s_maxItems = "maxItems";
        private const string s_uniqueItems = "uniqueItems";
        

        /// <inheritdoc />
        public IThingResponseBuilder SetThing(Thing thing)
        {
            _thing = thing;

            if (_option != null)
            {
                _thingName = _option.PropertyNamingPolicy.ConvertName(_thing.Name);
            }
            return this;
        }

        /// <inheritdoc />
        public IThingResponseBuilder SetThingOption(ThingOption option)
        {
            _option = option;
            
            if (_thing != null)
            {
                _thingName = _option.PropertyNamingPolicy.ConvertName(_thing.Name);
            }
            
            return this;
        }

        /// <inheritdoc />
        public void Add(EventInfo @event, ThingEventAttribute? eventInfo)
        {
            if (_thing == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThing)} before build");
            }
            
            if (_option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)} before add");
            }
            
            var information = new Dictionary<string, object?>();
            
            if (eventInfo != null)
            {
                if (!_option.IgnoreNullValues || eventInfo.Title != null)
                {
                    information.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Title)), eventInfo.Title);
                }

                if (!_option.IgnoreNullValues || eventInfo.Description != null)
                {
                    information.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Description)), eventInfo.Description);
                }

                if (!_option.IgnoreNullValues || eventInfo.Unit != null)
                {
                    information.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Unit)), eventInfo.Unit);
                }

                AddTypeProperty(information, eventInfo.Type);
            }
            
            var eventName = _option.PropertyNamingPolicy.ConvertName(eventInfo?.Name ?? @event.Name);
            
            information.Add(_option.PropertyNamingPolicy.ConvertName("Link"), new[]
            {
                new Link($"/things/{_thingName}/events/{eventName}", "event")
            });
            
            _events.Add(eventName, information);
        }

        /// <inheritdoc />
        public void Add(PropertyInfo property, ThingPropertyAttribute? attribute,  JsonSchema jsonSchema)
        {
            if (_thing == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThing)} before build");
            }
            
            if (_option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)} before add");
            }
            
            var propertyInformation = new Dictionary<string, object?>();
            
            if (!_option.IgnoreNullValues || attribute?.Title != null)
            {
                propertyInformation.Add(nameof(ThingEventAttribute.Title).ToLower(), attribute?.Title);
            }

            if (!_option.IgnoreNullValues || attribute?.Description != null)
            {
                propertyInformation.Add(nameof(ThingEventAttribute.Description).ToLower(), attribute?.Description);
            }

            if (!_option.IgnoreNullValues || attribute?.Unit != null)
            {
                propertyInformation.Add(nameof(ThingEventAttribute.Unit).ToLower(), attribute?.Unit);
            }
            
            AddTypeProperty(propertyInformation, attribute?.Type);

            AddInformation(propertyInformation, jsonSchema, property.PropertyType);
            var propertyName = _option.PropertyNamingPolicy.ConvertName(attribute?.Name ?? property.Name);
            
            propertyInformation.Add("link", new[]
            {
                new Link($"/things/{_thingName}/properties/{propertyName}", "property")
            });
            
            _properties.Add(propertyName, propertyInformation);
        }

        /// <inheritdoc />
        public void Add(MethodInfo action, ThingActionAttribute? attribute)
        {
            if (_thing == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThing)}");
            }
            
            if (_option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)}");
            }
            
            var propertyName = _option.PropertyNamingPolicy.ConvertName(attribute?.Name ?? action.Name);
            
            var actionInformation = new Dictionary<string, object?>();
            
            if (!_option.IgnoreNullValues || attribute?.Title != null)
            {
                actionInformation.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Title)), attribute?.Title);
            }

            if (!_option.IgnoreNullValues || attribute?.Description != null)
            {
                actionInformation.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Description)), attribute?.Description);
            }
            
            actionInformation.Add("link", new[]
            {
                new Link($"/things/{_thingName}/actions/{propertyName}", "action")
            });
            
            var input = new Dictionary<string, object?>();
            
            AddTypeProperty(input, attribute?.Type);
            
            input.Add("type", "object");
            
            _parameters = new Dictionary<string, object?>();
            
            input.Add("properties", _parameters);
            
            actionInformation.Add(_option.PropertyNamingPolicy.ConvertName("Input"), input);

            _actions.Add(propertyName, actionInformation);
        }

        /// <inheritdoc />
        public void Add(ParameterInfo parameter, ThingParameterAttribute? attribute, JsonSchema jsonSchema)
        {
            if (_thing == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThing)}");
            }
            
            if (_option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)}");
            }
            
            if (_parameters == null)
            {
                throw new InvalidOperationException($"Parameter is null, call {nameof(Add)}");
            }
            
            var parameterInformation = new Dictionary<string, object?>();
            
            if (!_option.IgnoreNullValues || attribute?.Title != null)
            {
                parameterInformation.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Title)), attribute?.Title);
            }

            if (!_option.IgnoreNullValues || attribute?.Description != null)
            {
                parameterInformation.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Description)), attribute?.Description);
            }

            if (!_option.IgnoreNullValues || attribute?.Unit != null)
            {
                parameterInformation.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Unit)), attribute?.Unit);
            }
            
            AddInformation(parameterInformation, jsonSchema, parameter.ParameterType);
            var parameterName = _option.PropertyNamingPolicy.ConvertName(attribute?.Name ?? parameter.Name);
            
            _parameters.Add(parameterName, parameterInformation);
        }

        private static void AddInformation(Dictionary<string, object?> builder, JsonSchema jsonSchema, Type type)
        {
            var jsonType = type.ToJsonType();
            builder.Add("type", jsonType.ToString().ToLower());

            if (jsonSchema.IsReadOnly.HasValue)
            {
                builder.Add(s_readOnly, jsonSchema.IsReadOnly);
            }
            
            if (jsonSchema.IsWriteOnly.HasValue)
            {
                builder.Add(s_writeOnly, jsonSchema.IsReadOnly);
            }
            
            if (jsonSchema.Enums != null)
            {
                builder.Add(s_enum, jsonSchema.Enums);
            }

            if (jsonSchema.Deprecated.HasValue)
            {
                builder.Add(s_deprecated, jsonSchema.Enums);
            }
            
            switch(jsonType)
            {
                case JsonType.String:
                    if (jsonSchema.MinimumLength.HasValue)
                    {
                        builder.Add(s_minLength, jsonSchema.MinimumLength);
                    }

                    if (jsonSchema.MaximumLength.HasValue)
                    {
                        builder.Add(s_maxLength, jsonSchema.MaximumLength);
                    }

                    if (jsonSchema.Pattern != null)
                    {
                        builder.Add(s_pattern, jsonSchema.Pattern);
                    }
                    break;
                case JsonType.Integer:
                case JsonType.Number:
                    if (jsonSchema.Minimum.HasValue)
                    {
                        builder.Add(s_min, jsonSchema.Minimum);
                    }

                    if (jsonSchema.Maximum.HasValue)
                    {
                        builder.Add(s_max, jsonSchema.Maximum);
                    }
                    
                    if (jsonSchema.ExclusiveMinimum.HasValue)
                    {
                        builder.Add(s_minExclusive, jsonSchema.ExclusiveMinimum);
                    }
                    
                    if (jsonSchema.ExclusiveMaximum.HasValue)
                    {
                        builder.Add(s_maxExclusive, jsonSchema.ExclusiveMaximum);
                    }
                    
                    if (jsonSchema.MultipleOf.HasValue)
                    {
                        builder.Add(s_multipleOf, jsonSchema.MultipleOf);
                    }
                    break;
                case JsonType.Array:
                    if (jsonSchema.MinimumItems.HasValue)
                    {
                        builder.Add(s_minItems, jsonSchema.Minimum);
                    }
                    
                    if (jsonSchema.MaximumItems.HasValue)
                    {
                        builder.Add(s_maxItems, jsonSchema.Minimum);
                    }
                    
                    if (jsonSchema.UniqueItems.HasValue)
                    {
                        builder.Add(s_uniqueItems, jsonSchema.Minimum);
                    }

                    var arrayType = type.GetCollectionType();
                    builder.Add(s_items, new Dictionary<string, object>
                    {
                        [s_type] = arrayType.ToJsonType().ToString().ToLower()
                    });
                    break;
                case JsonType.Boolean:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(jsonType), jsonType, null);
            }
        }
        
        private void AddTypeProperty(Dictionary<string, object?> builder, string[]? types)
        {
            if (_option == null)
            {
                throw new InvalidOperationException($"ThingOption is null, call {nameof(SetThingOption)} before add");
            }
            
            if (_option.IgnoreNullValues && types == null)
            {
                return;
            }
            
            if (types == null )
            {
                builder.Add("@type", null);
                return;
            }
            
            if (types.Length == 1)
            {
                builder.Add("@type", types[0]);
                return;
            }

            builder.Add("@type", types);
        }
        
        /// <inheritdoc />
        public Dictionary<string, object?> Build()
        {
            if (_thing == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThing)} before build");
            }

            if (_option == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThingOption)} before build");
            }

            var result = new Dictionary<string, object?>
            {
                ["@context"] = _thing.Context
            };

            if (!_option.IgnoreNullValues || _thing.Title != null)
            {
                result.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Thing.Title)), _thing.Title);
            }
            
            if (!_option.IgnoreNullValues || _thing.Description != null)
            {
                result.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Thing.Description)), _thing.Description);
            }
            
            AddTypeProperty(result, _thing.Type);

            if (_events.Any())
            {
                result.Add(_option.PropertyNamingPolicy.ConvertName("Events"), _events);
            }

            if (_properties.Any())
            {
                result.Add(_option.PropertyNamingPolicy.ConvertName("Properties"), _properties);
            }
            
            if (_actions.Any())
            {
                result.Add(_option.PropertyNamingPolicy.ConvertName("Actions"), _actions);
            }
            
            var links = new List<Link>(4)
            {
                new Link("properties", $"/things/{_thingName}/properties"),
                new Link("events", $"/things/{_thingName}/events"),
                new Link("actions", $"/things/{_thingName}/actions")
            };
            
            result.Add(_option.PropertyNamingPolicy.ConvertName("Links"), links);
            return result;
        }
    }
}
