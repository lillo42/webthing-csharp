using System;
using System.Collections.Generic;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing
{
    internal sealed class PropertyProxy : Property, IEquatable<Property>
    {
        private readonly Property _property;
        internal IJsonSchemaValidator SchemaValidator { get; set; }

        public PropertyProxy(Property property, IJsonSchemaValidator schemaValidator)
        {
            _property = property;
            SchemaValidator = schemaValidator;
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
                if (SchemaValidator.IsValid(value, _property.Metadata))
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Property property && !(obj is PropertyProxy))
            {
                return Equals(property);
            }

            return false;
        }

        public bool Equals(Property other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return ReferenceEquals(_property, other) || _property.Equals(other);
        }

        public override int GetHashCode()
            => (_property != null ? _property.GetHashCode() : 0);
        
    }
}
