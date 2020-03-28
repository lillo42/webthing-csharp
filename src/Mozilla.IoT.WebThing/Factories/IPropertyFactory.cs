using System;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Properties;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <summary>
    /// The factory of <see cref="IProperty"/>.
    /// </summary>
    public interface IPropertyFactory
    {
        /// <summary>
        /// Create new instance of <see cref="IProperty"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of property.</param>
        /// <param name="information">The <see cref="Information"/>.</param>
        /// <param name="thing"></param>
        /// <param name="setter"></param>
        /// <param name="getter"></param>
        /// <returns>New instance of <see cref="IProperty"/>.</returns>
        IProperty Create(Type propertyType, Information information, Thing thing, 
            Action<object, object?> setter, Func<object, object?> getter);
    }
}
