using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories.Generator.Converter;

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

        /// <inheritdoc />
        public IThingResponseBuilder SetThing(Thing thing)
        {
            _thing = thing;
            return this;
        }

        /// <inheritdoc />
        public IThingResponseBuilder SetThingOption(ThingOption option)
        {
            _option = option;
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

            var thingName = _option.PropertyNamingPolicy.ConvertName(_thing.Name);
            var eventName = _option.PropertyNamingPolicy.ConvertName(eventInfo?.Name ?? @event.Name);
            
            information.Add(_option.PropertyNamingPolicy.ConvertName("Link"), new[]
            {
                new Link($"/thing/{thingName}/events/{eventName}", "event")
            });
            
            _events.Add(eventName, information);
        }

        /// <inheritdoc />
        public void Add(PropertyInfo property, ThingPropertyAttribute? attribute,  Information information)
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
                propertyInformation.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Title)), attribute?.Title);
            }

            if (!_option.IgnoreNullValues || attribute?.Description != null)
            {
                propertyInformation.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Description)), attribute?.Description);
            }

            if (!_option.IgnoreNullValues || attribute?.Unit != null)
            {
                propertyInformation.Add(_option.PropertyNamingPolicy.ConvertName(nameof(ThingEventAttribute.Unit)), attribute?.Unit);
            }
            
            AddTypeProperty(propertyInformation, attribute?.Type);

            AddInformation(propertyInformation, information, ToJsonType(property.PropertyType), true);
            var thingName = _option.PropertyNamingPolicy.ConvertName(_thing.Name);
            var propertyName = _option.PropertyNamingPolicy.ConvertName(attribute?.Name ?? property.Name);
            
            propertyInformation.Add(_option.PropertyNamingPolicy.ConvertName("Link"), new[]
            {
                new Link($"/thing/{thingName}/properties/{propertyName}", "property")
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
            
            var thingName = _option.PropertyNamingPolicy.ConvertName(_thing.Name);
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
            
            actionInformation.Add(_option.PropertyNamingPolicy.ConvertName("Link"), new[]
            {
                new Link($"/thing/{thingName}/actions/{propertyName}", "action")
            });
            
            var input = new Dictionary<string, object?>();
            
            AddTypeProperty(input, attribute?.Type);
            
            input.Add("type", "object");
            
            _parameters = new Dictionary<string, object?>();
            
            input.Add(_option.PropertyNamingPolicy.ConvertName("Properties"), _parameters);
            
            actionInformation.Add(_option.PropertyNamingPolicy.ConvertName("Input"), input);

            _actions.Add(propertyName, actionInformation);
        }

        /// <inheritdoc />
        public void Add(ParameterInfo parameter, ThingParameterAttribute? attribute, Information information)
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
            
            AddInformation(parameterInformation, information, ToJsonType(parameter.ParameterType), false);
            var parameterName = _option.PropertyNamingPolicy.ConvertName(attribute?.Name ?? parameter.Name);
            
            _parameters.Add(parameterName, parameterInformation);
        }

        private void AddInformation(Dictionary<string, object?> builder, Information information, JsonType jsonType, bool writeIsReadOnlu)
        {
            builder.Add("type", jsonType.ToString().ToLower());
            
            if (_option == null)
            {
                throw new InvalidOperationException($"Thing is null, call {nameof(SetThingOption)} before build");
            }

            if (writeIsReadOnlu)
            {
                builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.IsReadOnly)), information.IsReadOnly);
            }
            
            switch(jsonType)
            {
                case JsonType.String:
                    if (!_option.IgnoreNullValues || information.MinimumLength.HasValue)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.MinimumLength)), information.MinimumLength);
                    }

                    if (!_option.IgnoreNullValues || information.MaximumLength.HasValue)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.MaximumLength)), information.MaximumLength);
                    }

                    if (!_option.IgnoreNullValues || information.Pattern != null)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.Pattern)), information.Pattern);
                    }

                    if (!_option.IgnoreNullValues || information.Enums != null)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.Enums)), information.Enums);
                    }
                    break;
                case JsonType.Integer:
                case JsonType.Number:
                    if (!_option.IgnoreNullValues ||  information.Minimum.HasValue)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.Minimum)), information.Minimum);
                    }

                    if (!_option.IgnoreNullValues || information.Maximum.HasValue)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.Maximum)), information.Maximum);
                    }
                    
                    if (!_option.IgnoreNullValues || information.ExclusiveMinimum.HasValue)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.ExclusiveMinimum)), information.ExclusiveMinimum);
                    }
                    
                    if (!_option.IgnoreNullValues || information.ExclusiveMaximum.HasValue)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.ExclusiveMaximum)), information.ExclusiveMaximum);
                    }
                    
                    if (!_option.IgnoreNullValues || information.MultipleOf.HasValue)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.MultipleOf)), information.MultipleOf);
                    }
                    
                    if (!_option.IgnoreNullValues || information.Enums != null)
                    {
                        builder.Add(_option.PropertyNamingPolicy.ConvertName(nameof(Information.Enums)), information.Enums);
                    }
                    break;
                case JsonType.Array:
                case JsonType.Boolean:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(jsonType), jsonType, null);
            }
        }

        private static JsonType ToJsonType(Type type)
        {
             type = type.GetUnderlyingType();
             
             if (type == typeof(string)
                 || type == typeof(char)
                 || type == typeof(DateTime)
                 || type == typeof(DateTimeOffset)
                 || type == typeof(Guid)
                 || type == typeof(TimeSpan)
                 || type.IsEnum)
             {
                 return JsonType.String;
             }

             if (type == typeof(bool))
             {
                 return JsonType.Boolean;
             }
            
             if (type == typeof(int)
                 || type == typeof(sbyte)
                 || type == typeof(byte)
                 || type == typeof(short)
                 || type == typeof(long)
                 || type == typeof(uint)
                 || type == typeof(ulong)
                 || type == typeof(ushort))
             {
                 return JsonType.Integer;
             }
            
             if (type == typeof(double)
                 || type == typeof(float)
                 || type == typeof(decimal))
             {
                 return JsonType.Number;
             }
             
             return JsonType.Array;
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

            return result;
        }
    }
}
