using System;
using System.Diagnostics.CodeAnalysis;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Mozilla.IoT.WebThing.Json.SchemaValidations;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// Represent the thing property.
    /// </summary>
    public class ThingProperty : IThingProperty
    {
        private readonly Thing _thing;

        private readonly IJsonConvertible? _jsonConvertible;
        private readonly IJsonSchemaValidation? _jsonSchemaValidation;
        private readonly IConvertible? _convertible;
        
        private readonly Action<Thing, object?>? _setter;
        private readonly Func<Thing, object?> _getter;
        
        private readonly bool _isReadOnly;
        private readonly bool _isWriteOnly;

        /// <summary>
        /// Initialize a new instance of <see cref="ThingProperty"/>.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="isWriteOnly"></param>
        /// <param name="setter"></param>
        /// <param name="getter"></param>
        /// <param name="jsonSchemaValidation"></param>
        /// <param name="convertible"></param>
        /// <param name="jsonConvertible"></param>
        /// <param name="originPropertyName"></param>
        public ThingProperty(Thing thing, bool isReadOnly, bool isWriteOnly, 
            Func<Thing, object?> getter, Action<Thing, object?>? setter, 
            IJsonSchemaValidation? jsonSchemaValidation, IJsonConvertible? jsonConvertible, 
            IConvertible? convertible, string originPropertyName)
        {
            if (originPropertyName == null)
            {
                throw new ArgumentNullException(nameof(originPropertyName));
            }

            _thing = thing ?? throw new ArgumentNullException(nameof(thing));

            _isReadOnly = isReadOnly;
            _isWriteOnly = isWriteOnly;
            
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            
            if (!isReadOnly)
            {
                if (setter == null)
                {
                    throw new ArgumentNullException(nameof(setter));
                }
                
                if (jsonSchemaValidation == null)
                {
                    throw new ArgumentNullException(nameof(jsonSchemaValidation));
                }
                
                if (jsonConvertible == null)
                {
                    throw new ArgumentNullException(nameof(jsonConvertible));
                }
            }
            
            _setter = setter;
            
            _jsonSchemaValidation = jsonSchemaValidation;
            _jsonConvertible = jsonConvertible;
            _convertible = convertible;
            OriginPropertyName = originPropertyName;
        }

        /// <inheritdoc />
        public string OriginPropertyName { get; }

        /// <inheritdoc />
        public bool TryGetValue([MaybeNull]out object? value)
        {
            if (_isWriteOnly)
            {
                value = null;
                return false;
            }

            value = _getter(_thing);
            return true;
        }

        /// <inheritdoc />
        public SetPropertyResult TrySetValue(object value)
        {
            if (_isReadOnly)
            {
                return SetPropertyResult.ReadOnly;
            }

            if (!_jsonConvertible!.TryConvert(value, out var jsonValue))
            {
                return SetPropertyResult.InvalidValue;
            }

            if (!_jsonSchemaValidation!.IsValid(jsonValue))
            {
                return SetPropertyResult.InvalidValue;
            }

            _setter!(_thing, _convertible == null ? jsonValue : _convertible.Convert(jsonValue));
            return SetPropertyResult.Ok;
        }
    }
}
