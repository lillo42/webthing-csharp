using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonSchemaValidator
    {
        bool IsValid(object value, IDictionary<string, object> schema);
    }
}
