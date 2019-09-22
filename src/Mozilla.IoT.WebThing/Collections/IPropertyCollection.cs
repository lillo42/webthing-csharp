using System;
using System.Collections.Generic;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.Collections
{
    public interface IPropertyCollection : ICollection<Property>
    {
        IJsonSchemaValidator JsonSchemaValidator { get; set; }
        IJsonValue JsonValue { get; set; }
        void SetProperty(string propertyName, object value);

        event EventHandler<ValueChangedEventArgs> ValueChanged;
    }
}
