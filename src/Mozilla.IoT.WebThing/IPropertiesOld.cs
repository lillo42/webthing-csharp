using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    public interface IPropertiesOld
    {
        Dictionary<string, object>? GetProperties(string? propertyName = null);

        SetPropertyResult SetProperty(string propertyName, object value);
    }
}
