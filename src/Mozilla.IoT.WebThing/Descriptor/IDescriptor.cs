using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Description
{
    public interface IDescriptor<T>
    {
        IDictionary<string, object> CreateDescription(T value);
    }
}