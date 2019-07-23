using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Description
{
    public interface IDescription<T>
    {
        IDictionary<string, object> CreateDescription(T value);
    }
}