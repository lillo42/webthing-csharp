using System;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Builders;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <summary>
    /// The factory of <see cref="IActionParameter"/>.
    /// </summary>
    public interface IActionParameterFactory
    {
        /// <summary>
        /// Create new instance of <see cref="IActionParameter"/>.
        /// </summary>
        /// <param name="parameterType">The <see cref="Type"/> of parameter.</param>
        /// <param name="information">The <see cref="Information"/>.</param>
        /// <returns>Return new instance of <see cref="IActionParameter"/>.</returns>
        IActionParameter Create(Type parameterType, Information information);
    }
}
