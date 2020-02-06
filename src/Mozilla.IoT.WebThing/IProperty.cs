using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    public interface IProperty
    {
        bool ContainsProperty(string propertyName);
        object GetProperty(string propertyName);
        Dictionary<string, object> GetProperties();
        SetPropertyResult SetProperty(string propertyName, object value);
    }
}
