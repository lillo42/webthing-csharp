using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Descriptor
{
    public interface IDescriptor<T>
    {
        IDictionary<string, object> CreateDescription(T value);
    }
}