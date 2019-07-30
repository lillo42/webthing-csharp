using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Collections
{
    public interface IThingReadOnlyCollection : IReadOnlyCollection<Thing>
    {
        Thing this[string name] { get; }
    }
}
