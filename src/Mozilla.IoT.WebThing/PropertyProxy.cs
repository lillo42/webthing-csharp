using System.Collections.Generic;
using System.Diagnostics;
using Mozilla.IoT.WebThing.Collections;
using Mozilla.IoT.WebThing.DebugView;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing
{
    [DebuggerTypeProxy(typeof (PropertyProxyDebugView))]
    [DebuggerDisplay("Value = {Value}")]
    internal sealed class PropertyProxy : Property
    {
        internal IJsonSchemaValidator SchemaValidator { get; set; }

        internal Property Property {get; }

        public PropertyProxy(Property property, IJsonSchemaValidator schemaValidator)
        {
            Property = property;
            SchemaValidator = schemaValidator;
        }

        public override Thing Thing
        {
            get => Property.Thing;
            set => Property.Thing = value;
        }

        public override string Name
        {
            get => Property.Name;
            set => Property.Name = value;
        }

        public override string Href => Property.Href;

        public override string HrefPrefix
        {
            get => Property.HrefPrefix;
            set => Property.HrefPrefix = value;
        }

        public override object Value
        {
            get => Property.Value;
            set
            {
                if (SchemaValidator.IsValid(value, Property.Metadata))
                {
                    Property.Value = value;
                }
            }
        }

        public override IDictionary<string, object> Metadata
        {
            get => Property.Metadata;
            set => Property.Metadata = value;
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

            if (obj is Property property)
            {
                return Equals(property);
            }

            return false;
        }

        public override int GetHashCode()
            => (Property != null ? Property.GetHashCode() : 0);
    }
}
