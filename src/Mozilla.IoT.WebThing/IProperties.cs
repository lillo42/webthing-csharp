using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    public interface IProperties
    {
        Dictionary<string, object>? GetProperties(string? propertyName = null);
    }
}
