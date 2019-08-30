using System;

namespace Mozilla.IoT.WebThing.Json
{
    public interface IJsonValue
    {
        object GetValue(object value, Type type);
    }
}
