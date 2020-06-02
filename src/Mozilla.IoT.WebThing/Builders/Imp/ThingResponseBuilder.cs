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

        private const string s_link = "links";
        private const string s_title = "title";
        private const string s_description = "description";
        private const string s_unit = "unit";
        
        private const string s_items = "items";
        private const string s_type = "type";
        private const string s_input = "input";
        private const string s_properties = "properties";
        
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

        private const string s_security = "security";
        private const string s_securityDefinitions = "securityDefinitions";

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
            
            var eventName = _option.PropertyNamingPolicy.ConvertName(eventInfo?.Name ?? @event.Name);
            var information = new Dictionary<string, object?>();
            AddSchemaInformation(information, eventInfo!.ToJsonSchema(@event), @event.EventHandlerType!.GetGenericArguments()[0]);
            information.Add(s_link, new[]
            {
                new Link("event", $"/things/{_thingName}/events/{eventName}")
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
            
            AddSchemaInformation(propertyInformation, jsonSchema, property.PropertyType);
            
            var propertyName = _option.PropertyNamingPolicy.ConvertName(attribute?.Name ?? property.Name);
            
            propertyInformation.Add(s_link, new[]
            {
                new Link("property", $"/things/{_thingName}/properties/{propertyName}")
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
            
            AddSchemaInformation(actionInformation, attribute!.ToJsonSchema(action), null);
            
            actionInformation.Add(s_link, new[]
            {
                new Link("action", $"/things/{_thingName}/actions/{propertyName}")
            });

            var input = new Dictionary<string, object?>
            {
                [s_type] = "object"
            };

            _parameters = new Dictionary<string, object?>();
            input.Add(s_properties, _parameters);
            AddTypeProperty(input, attribute?.Type);
            actionInformation.Add(s_input, input);
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
            
            AddSchemaInformation(parameterInformation, jsonSchema, parameter.ParameterType);
            var parameterName = _option.PropertyNamingPolicy.ConvertName(attribute?.Name ?? parameter.Name);
            
            _parameters.Add(parameterName, parameterInformation);
        }

        private static void AddSchemaInformation(Dictionary<string, object?> builder, JsonSchema jsonSchema, Type? type)
        {
            if (!string.IsNullOrEmpty(jsonSchema.Title))
            {
                builder.Add(s_title, jsonSchema.Title);
            }

            if (!string.IsNullOrEmpty(jsonSchema.Description))
            {
                builder.Add(s_description, jsonSchema.Description);
            }

            if (!string.IsNullOrEmpty(jsonSchema.Unit))
            {
                builder.Add(s_unit, jsonSchema.Unit);
            }

            if (type == null)
            {
                return;
            }
            
            if (jsonSchema.Type != null && jsonSchema.Type.Length > 0)
            {
                if (jsonSchema.Type.Length == 1)
                {
                    builder.Add("@type", jsonSchema.Type[0]);
                }
                else
                {
                    
                    builder.Add("@type", jsonSchema.Type);
                }
            }
            
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

            if (jsonSchema.Deprecated.HasValue)
            {
                builder.Add(s_deprecated, jsonSchema.Enums);
            }
            
            switch(jsonType)
            {
                case JsonType.String:
                    if (jsonSchema.Enums != null)
                    {
                        builder.Add(s_enum, jsonSchema.Enums);
                    }
                    
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
                    if (jsonSchema.Enums != null)
                    {
                        builder.Add(s_enum, jsonSchema.Enums);
                    }
                    
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
                        builder.Add(s_minItems, jsonSchema.MinimumItems);
                    }
                    
                    if (jsonSchema.MaximumItems.HasValue)
                    {
                        builder.Add(s_maxItems, jsonSchema.MaximumItems);
                    }
                    
                    if (jsonSchema.UniqueItems.HasValue)
                    {
                        builder.Add(s_uniqueItems, jsonSchema.UniqueItems);
                    }
                    
                    var arrayType = type.GetCollectionType();
                    var items = new Dictionary<string, object>
                    {
                        [s_type] = arrayType.ToJsonType().ToString().ToLower()
                    };
                    
                    if (jsonSchema.Enums != null)
                    {
                        items.Add(s_enum, jsonSchema.Enums);
                    }
                    
                    builder.Add(s_items, items);
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
            
            if (types == null || types.Length == 0)
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

            if (_thing.Title != null)
            {
                result.Add(s_title, _thing.Title);
            }
            
            if (_thing.Description != null)
            {
                result.Add(s_description, _thing.Description);
            }
            
            if (_thing.Security != null)
            {
                result.Add(s_security, _thing.Security);
            }
            
            if (_thing.SecurityDefinitions?.Count > 0)
            {
                result.Add(s_securityDefinitions, _thing.SecurityDefinitions);
            }
            
            AddTypeProperty(result, _thing.Type);

            if (_events.Any())
            {
                result.Add("events", _events);
            }

            if (_properties.Any())
            {
                result.Add(s_properties, _properties);
            }
            
            if (_actions.Any())
            {
                result.Add("actions", _actions);
            }
            
            var links = new List<Link>(4)
            {
                new Link("properties", $"/things/{_thingName}/properties"),
                new Link("actions", $"/things/{_thingName}/actions"),
                new Link("events", $"/things/{_thingName}/events")
            };
            
            result.Add(s_link, links);
            return result;
        }
    }
}
