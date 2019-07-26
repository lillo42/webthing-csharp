using System.Collections.Generic;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing
{
    internal sealed class PropertyProxy : Property
    {
        private readonly Property _property;
        private readonly IJsonSchemaValidator _schemaValidator;

        public PropertyProxy(Thing thing, Property property, IJsonSchemaValidator schemaValidator)
        {
            _property = property;
            _schemaValidator = schemaValidator;
            _property.Thing = thing;
        }

        public override Thing Thing
        {
            get => _property.Thing;
            set => _property.Thing = value;
        }

        public override string Name
        {
            get => _property.Name;
            set => _property.Name = value;
        }

        public override string Href
        {
            get => _property.Href;
            set => _property.Href = value;
        }

        public override string HrefPrefix
        {
            get => _property.HrefPrefix;
            set => _property.HrefPrefix = value;
        }

        public override object Value
        {
            get => _property.Value;
            set
            {
                if (_schemaValidator.IsValid(value, _property.Metadata))
                {
                    _property.Value = value;
                }
            }
        }

        public override IDictionary<string, object> Metadata
        {
            get => _property.Metadata;
            set => _property.Metadata = value;
        }
    }
    
    
    internal sealed class PropertyProxy<T> : Property<T>
    {
        private readonly Property<T> _property;
        private readonly IJsonSchemaValidator _schemaValidator;

        public PropertyProxy(Thing thing, Property<T> property, IJsonSchemaValidator schemaValidator)
        {
            _property = property;
            _schemaValidator = schemaValidator;
            _property.Thing = thing;
        }

        public override Thing Thing
        {
            get => _property.Thing;
            set => _property.Thing = value;
        }

        public override string Name
        {
            get => _property.Name;
            set => _property.Name = value;
        }

        public override string Href
        {
            get => _property.Href;
            set => _property.Href = value;
        }

        public override string HrefPrefix
        {
            get => _property.HrefPrefix;
            set => _property.HrefPrefix = value;
        }

        public override T Value
        {
            get => _property.Value;
            set
            {
                if (_schemaValidator.IsValid(value, _property.Metadata))
                {
                    _property.Value = value;
                }
            }
        }

        public override IDictionary<string, object> Metadata
        {
            get => _property.Metadata;
            set => _property.Metadata = value;
        }
    }
}
