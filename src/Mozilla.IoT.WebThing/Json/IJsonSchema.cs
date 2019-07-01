using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonSchema
    {
        bool IsReadOnly { get; }
        bool IsValid(object value);
    }
}
