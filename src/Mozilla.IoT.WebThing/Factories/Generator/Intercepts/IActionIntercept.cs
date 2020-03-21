using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    /// <summary>
    /// Intercept <see cref="Thing"/> Method.
    /// </summary>
    public interface IActionIntercept : IIntercept
    {
        /// <summary>
        /// Intercept <see cref="Thing"/> Method.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="action">The method intercept.</param>
        /// <param name="actionInformation">The <see cref="ThingActionAttribute"/>.</param>
        void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInformation);
    }
}
