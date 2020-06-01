using System;
using Mozilla.IoT.WebThing.Json.Convertibles;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <summary>
    /// The factory of <see cref="IJsonConvertible"/>.
    /// </summary>
    public interface IJsonConvertibleFactory
    {
        /// <summary>
        /// Create new instance of <see cref="IJsonConvertible"/> for <see cref="bool"/>.
        /// </summary>
        /// <returns>New instance of <see cref="IJsonConvertible"/>.</returns>
        IJsonConvertible Create(TypeCode typeCode, Type type);
    }
}
