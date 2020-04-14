using System;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <summary>
    /// The factory of <see cref="Convertibles.IConvertible"/>.
    /// </summary>
    public interface IConvertibleFactory
    {
        /// <summary>
        /// Create new instance of <see cref="Convertibles.IConvertible"/> for <see cref="bool"/>.
        /// </summary>
        /// <returns>New instance of <see cref="Convertibles.IConvertible"/>.</returns>
        IConvertible? Create(TypeCode typeCode, Type type);
    }
}
